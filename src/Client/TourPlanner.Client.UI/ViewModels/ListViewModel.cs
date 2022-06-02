using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        private Tour? selectedItem;
        public Tour? SelectedTour
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                _tourSelectionService.Tour = value;
            }
        }

        public ICommand AddListPoint { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }

        private readonly ITourCollectionService _tourCollectionService;
        private readonly ITourSelectionService _tourSelectionService;
        private readonly ITourReportGenerationService _tourReportGenerationService;
        private readonly ISaveFileDialogProvider _saveFileDialogProvider;

        public ObservableCollection<Tour> Tours
        {
            get
            {
                return _tourCollectionService.DisplayedTours;
            }
        }

        public ListViewModel(
            ITourCollectionService tourCollectionService,
            ITourSelectionService tourSelectionService,
            ITourReportGenerationService tourReportGenerationService,
            ISaveFileDialogProvider saveFileDialogProvider)
        {
            _tourCollectionService = tourCollectionService;
            _tourSelectionService = tourSelectionService;
            _tourReportGenerationService = tourReportGenerationService;
            _saveFileDialogProvider = saveFileDialogProvider;

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
            DeleteTourCommand = new RelayCommand(DeleteTour);
            GenerateTourReportCommand = new RelayCommand(GenerateTourReport);
        }

        private void DeleteTour(object? obj)
        {
            if (SelectedTour != null && (!_tourCollectionService.Online || _tourCollectionService.DeleteTourApi(SelectedTour.Id)))
            {
                _tourCollectionService.AllTours.Remove(SelectedTour);
            }
        }


        private void GenerateTourReport(object? obj)
        {
            if (SelectedTour != null)
            {
                try
                {
                    _saveFileDialogProvider.DefaultExt = ".pdf";
                    _saveFileDialogProvider.Filter = "Pdf documents (.pdf)|*.pdf";

                    var fileName = _saveFileDialogProvider.GetFileName();

                    if (fileName != null)
                    {
                        var report = _tourReportGenerationService.GenerateReport(SelectedTour);
                        File.WriteAllBytes(fileName, report);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
