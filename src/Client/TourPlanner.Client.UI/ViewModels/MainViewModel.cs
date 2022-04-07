using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

        public ICommand SearchCommand { get; set; }

        private readonly ITourCollectionService _tourCollectionService;

        public MainViewModel(ITourCollectionService tourCollectionService)
        {
            _tourCollectionService = tourCollectionService;

            SearchCommand = new RelayCommand(Search);

            // Get tours from API and close application if it cannot be retrieved
            if (!_tourCollectionService.LoadToursApi())
            {
                Application.Current.Shutdown();
            }
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
