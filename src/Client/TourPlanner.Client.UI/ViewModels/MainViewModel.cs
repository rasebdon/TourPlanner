using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public string SearchText { get; set; } = string.Empty;

        public ICommand SearchCommand { get; }

        // Hotbar Commands
        public ICommand ExitCommand { get; }
        public ICommand ExportCommand { get; }

        private readonly ITourCollectionService _tourCollectionService;

        public MainViewModel(ITourCollectionService tourCollectionService)
        {
            _tourCollectionService = tourCollectionService;

            SearchCommand = new RelayCommand(Search);
            ExitCommand = new RelayCommand(Exit);
            ExportCommand = new RelayCommand(Export);

            // Get tours from API and close application if it cannot be retrieved
            if (!_tourCollectionService.LoadToursApi())
            {
                Exit(null);
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
