using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        public Tour? SelectedItem { get; set; }

        public ICommand AddListPoint { get; }
        public ICommand RemoveListPoint { get; }

        private ITourCollectionService _tourCollectionService;

        public ObservableCollection<Tour> Tours
        {
            get
            {
                return _tourCollectionService.Tours;
            }
        }

        public ListViewModel(ITourCollectionService tourCollectionService)
        {
            _tourCollectionService = tourCollectionService;

            // TODO : Open a new window for creating a new tour
            AddListPoint = new RelayCommand(
                o =>
                {
                    var tour = new Tour()
                    {
                        Id = -1,
                        Name = "New Tour",
                        EndPoint = new()
                        {
                            Latitude = 39,
                            Longitude = 39.05f,
                        },
                        StartPoint = new()
                        {
                            Latitude = 39.095f,
                            Longitude = 39.15f,
                        },
                        TransportType = TransportType.AUTO,
                        Entries = new()
                    };
                    if(_tourCollectionService.SaveTourApi(ref tour))
                        _tourCollectionService.Tours.Add(tour);

                },
                o => true);
            RemoveListPoint = new RelayCommand(
                o =>
                {
                    if (SelectedItem != null)
                    {
                        if (_tourCollectionService.DeleteTourApi(SelectedItem.Id))
                            _tourCollectionService.Tours.Remove(SelectedItem);
                    }
                },
                o => true);
        }
    }
}
