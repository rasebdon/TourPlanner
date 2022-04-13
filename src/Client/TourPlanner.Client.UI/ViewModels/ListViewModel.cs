using System.Collections.ObjectModel;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        private Tour? selectedItem;
        public Tour? SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;

                // Change other component views
                _logViewModel.Tour = selectedItem;
                _tourViewModel.Tour = selectedItem;
                _mainViewModel.Tour = selectedItem;
            }
        }

        public ICommand AddListPoint { get; }
        public ICommand RemoveListPoint { get; }

        private readonly ITourCollectionService _tourCollectionService;
        private readonly LogViewModel _logViewModel;
        private readonly TourViewModel _tourViewModel;
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<Tour> Tours
        {
            get
            {
                return _tourCollectionService.DisplayedTours;
            }
        }

        public ListViewModel(
            ITourCollectionService tourCollectionService,
            LogViewModel logViewModel,
            TourViewModel tourViewModel,
            MainViewModel mainViewModel)
        {
            _tourCollectionService = tourCollectionService;
            _logViewModel = logViewModel;
            _tourViewModel = tourViewModel;
            _mainViewModel = mainViewModel;

            AddListPoint = new RelayCommand(
                o =>
                {
                    var popup = new NewTourWindow();
                    popup.ShowDialog();
                },
                o => true);
            RemoveListPoint = new RelayCommand(
                o =>
                {
                    if (SelectedItem != null)
                    {
                        if (!_tourCollectionService.Online || _tourCollectionService.DeleteTourApi(SelectedItem.Id))
                        {
                            _tourCollectionService.AllTours.Remove(SelectedItem);
                        }
                    }
                },
                o => true);
        }
    }
}
