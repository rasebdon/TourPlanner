using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Client.UI.Views;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public string SearchText { get; set; } = string.Empty;

        public ICommand SearchCommand { get; }

        // Hotbar Commands
        public ICommand ExitCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummarizeReportCommand { get; }

#if DEBUG
        public ICommand CreateDummyData { get; }
#endif

        public Tour? Tour { get; set; }

        private readonly ITourCollectionService _tourCollectionService;
        private readonly ITourReportGenerationService _tourReportGenerationService;
        private readonly ISummarizeReportGenerationService _summarizeReportGenerationService;

        public MainViewModel(
            ITourCollectionService tourCollectionService,
            ITourReportGenerationService tourReportGenerationService,
            ISummarizeReportGenerationService summarizeReportGenerationService)
        {
            _tourCollectionService = tourCollectionService;
            _tourReportGenerationService = tourReportGenerationService;
            _summarizeReportGenerationService = summarizeReportGenerationService;

            SearchCommand = new RelayCommand(Search);
            ExitCommand = new RelayCommand(Exit);
            ImportCommand = new RelayCommand(Import);
            ExportCommand = new RelayCommand(Export);
            AboutCommand = new RelayCommand(About);
            SettingsCommand = new RelayCommand(Settings);
            GenerateTourReportCommand = new RelayCommand(GenerateTourReport, 
                o =>
                {
                    return Tour != null;
                });
            GenerateSummarizeReportCommand = new RelayCommand(GenerateSummarizeReport,
                o =>
                {
                    return _tourCollectionService.AllTours.Count > 0;
                });

            this.Tour = null;
#if DEBUG
            CreateDummyData = new RelayCommand(
                (object? o) =>
                {
                    ((TourCollectionService)_tourCollectionService).PostDummyData();
                });
#endif

            // Try to get tours from API
            if (!_tourCollectionService.LoadToursApi())
            {
                MessageBox.Show(
                    "Tours are not saved online!",
                    "(Unsafe) Offline mode",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void GenerateSummarizeReport(object? obj)
        {
            if (_tourCollectionService.AllTours.Count > 0)
            {
                try
                {
                    // Create SaveFileDialog
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new();

                    saveFileDialog.DefaultExt = ".pdf";
                    saveFileDialog.Filter = "Pdf documents (.pdf)|*.pdf";
                    bool? result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        var report = _summarizeReportGenerationService.GenerateReport(
                            _tourCollectionService.AllTours);
                        File.WriteAllBytes(saveFileDialog.FileName, report);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateTourReport(object? obj)
        {
            if (Tour != null)
            {
                try
                {
                    // Create SaveFileDialog
                    Microsoft.Win32.SaveFileDialog saveFileDialog = new();

                    saveFileDialog.DefaultExt = ".pdf";
                    saveFileDialog.Filter = "Pdf documents (.pdf)|*.pdf";
                    bool? result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        var report = _tourReportGenerationService.GenerateReport(Tour);
                        File.WriteAllBytes(saveFileDialog.FileName, report);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Settings(object? obj)
        {
            SettingsView settingsView = new();

            settingsView.ShowDialog();
        }

        private void About(object? obj)
        {
            AboutView aboutView = new();

            aboutView.ShowDialog();
        }

        private void Import(object? obj)
        {
            try
            {
                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog openFileDialog = new();

                openFileDialog.DefaultExt = ".tours";
                openFileDialog.Filter = "Tour planner documents (.tours)|*.tours";
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    _tourCollectionService.Import(new Uri(openFileDialog.FileName, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Export(object? obj)
        {
            try
            {
                // Create SaveFileDialog
                Microsoft.Win32.SaveFileDialog saveFileDialog = new();

                saveFileDialog.DefaultExt = ".tours";
                saveFileDialog.Filter = "Tour planner documents (.tours)|*.tours";
                bool? result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    _tourCollectionService.Export(new Uri(saveFileDialog.FileName, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit(object? obj)
        {
            Application.Current.Shutdown();
        }

        private void Search(object? obj)
        {
            // Create a collection with only tours that match the search string
            var tours = _tourCollectionService.AllTours
                .Where(e => e.Description.Contains(SearchText) || e.Name.Contains(SearchText)).ToList();

            _tourCollectionService.DisplayedTours.Clear();
            tours.ForEach(e => _tourCollectionService.DisplayedTours.Add(e));
        }
    }
}
