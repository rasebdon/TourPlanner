using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Views;

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

#if DEBUG
        public ICommand CreateDummyData { get; }
#endif
        private readonly ITourCollectionService _tourCollectionService;

        public MainViewModel(ITourCollectionService tourCollectionService)
        {
            _tourCollectionService = tourCollectionService;

            SearchCommand = new RelayCommand(Search);
            ExitCommand = new RelayCommand(Exit);
            ImportCommand = new RelayCommand(Import);
            ExportCommand = new RelayCommand(Export);
            AboutCommand = new RelayCommand(About);
            SettingsCommand = new RelayCommand(Settings);

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
                Nullable<bool> result = openFileDialog.ShowDialog();
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
                // Create OpenFileDialog
                Microsoft.Win32.SaveFileDialog saveFileDialog = new();

                saveFileDialog.DefaultExt = ".tours";
                saveFileDialog.Filter = "Tour planner documents (.tours)|*.tours";
                Nullable<bool> result = saveFileDialog.ShowDialog();
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
