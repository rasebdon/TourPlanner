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

        public string Title
        {
            get
            {
                if (_tourCollectionService.Online)
                    return "Tour Planner";
                else return "Tour Planner (Offline)";
            }
        }

        public ICommand SearchCommand { get; }

        // Hotbar Commands
        public ICommand ExitCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummarizeReportCommand { get; }

#if DEBUG
        public ICommand CreateDummyData { get; }
#endif

        public Tour? Tour { get; set; }

        private readonly ITourCollectionService _tourCollectionService;
        private readonly ITourReportGenerationService _tourReportGenerationService;
        private readonly ISummarizeReportGenerationService _summarizeReportGenerationService;
        private readonly ISaveFileDialogProvider _saveFileDialogProvider;
        private readonly IOpenFileDialogProvider _openFileDialogProvider;

        public MainViewModel(
            ITourCollectionService tourCollectionService,
            ITourReportGenerationService tourReportGenerationService,
            ISummarizeReportGenerationService summarizeReportGenerationService,
            ISaveFileDialogProvider saveFileDialogProvider,
            IOpenFileDialogProvider openFileDialogProvider)
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
            DeleteTourCommand = new RelayCommand(DeleteTour,
                o =>
                {
                    return Tour != null;
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
#if !DEBUG
                MessageBox.Show(
                    "Could not connect to server!\nSome features are disabled!",
                    "Offline mode",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
#endif
            }
            _saveFileDialogProvider = saveFileDialogProvider;
            _openFileDialogProvider = openFileDialogProvider;
        }

        private void DeleteTour(object? obj)
        {
            if (Tour != null && (!_tourCollectionService.Online || _tourCollectionService.DeleteTourApi(Tour.Id)))
            {
                _tourCollectionService.AllTours.Remove(Tour);
            }
        }

        private void GenerateSummarizeReport(object? obj)
        {
            if (_tourCollectionService.AllTours.Count > 0)
            {
                try
                {
                    _saveFileDialogProvider.DefaultExt = ".pdf";
                    _saveFileDialogProvider.Filter = "Pdf documents (.pdf)|*.pdf";

                    var fileName = _saveFileDialogProvider.GetFileName();
                    if (fileName != null)
                    {
                        var report = _summarizeReportGenerationService.GenerateReport(
                            _tourCollectionService.AllTours);
                        File.WriteAllBytes(fileName, report);
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
                    _saveFileDialogProvider.DefaultExt = ".pdf";
                    _saveFileDialogProvider.Filter = "Pdf documents (.pdf)|*.pdf";

                    var fileName = _saveFileDialogProvider.GetFileName();

                    if (fileName != null)
                    {
                        var report = _tourReportGenerationService.GenerateReport(Tour);
                        File.WriteAllBytes(fileName, report);
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
                _openFileDialogProvider.DefaultExt = ".tours";
                _openFileDialogProvider.Filter = "Tour planner documents (.tours)|*.tours";

                var fileName = _openFileDialogProvider.GetFileName();

                if (fileName != null)
                {
                    _tourCollectionService.Import(new Uri(fileName, UriKind.Absolute));
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
                _saveFileDialogProvider.DefaultExt = ".tours";
                _saveFileDialogProvider.Filter = "Tour planner documents (.tours)|*.tours";

                var fileName = _saveFileDialogProvider.GetFileName();

                if (fileName != null)
                {
                    _tourCollectionService.Export(new Uri(fileName, UriKind.Absolute));
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
