using System.Collections.ObjectModel;
using System.Windows;
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
        public ICommand DeleteTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }

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
                    if(_tourCollectionService.Online)
                    {
                        var popup = new NewTourWindow();
                        popup.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Cannot add tour in offline mode!",
                            "Offline mode",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                },
                o => true);
            DeleteTourCommand = _mainViewModel.DeleteTourCommand;
            GenerateTourReportCommand = _mainViewModel.GenerateTourReportCommand;
        }
    }
}
