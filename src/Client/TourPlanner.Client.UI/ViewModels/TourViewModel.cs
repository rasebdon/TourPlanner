using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class TourViewModel : BaseViewModel
    {
        public BitmapImage TourImage { get; set; }

        private Tour? _tour;
        public Tour? Tour
        {
            get
            {
                return _tour;
            }
            set
            {
                _tour = value;
                if (_tour != null)
                {
                    if (_tourCollectionService.Online)
                        UpdateMap(_tour, false);
                }
                else
                {
                    TourImage = _bitmapImageService.ToBitmapImage(_tourImageService.DefaultImage);
                }
                OnPropertyChanged(nameof(Tour));
                OnPropertyChanged(nameof(Tour.Description));
                OnPropertyChanged(nameof(Tour.Distance));
                OnPropertyChanged(nameof(Tour.ChildFriendliness));
                OnPropertyChanged(nameof(Tour.Popularity));
                OnPropertyChanged(nameof(Tour.TransportType));
                OnPropertyChanged(nameof(TourImage));
            }
        }

        public Visibility ImageEnabled { get; set; } = Visibility.Visible;
        public Visibility DescriptionEnabled { get; set; } = Visibility.Hidden;

        public ICommand RefreshImageCommand { get; }

        public ICommand DisplayRoute { get; }
        public ICommand DisplayDescription { get; }

        private readonly ITourCollectionService _tourCollectionService;
        private readonly ITourImageService _tourImageService;
        private readonly IBitmapImageService _bitmapImageService;

        public TourViewModel(
            ITourCollectionService tourCollectionService,
            ITourImageService tourImageService,
            IBitmapImageService bitmapImageService)
        {
            _tourCollectionService = tourCollectionService;
            _tourImageService = tourImageService;
            _bitmapImageService = bitmapImageService;

            DisplayRoute = new RelayCommand(ShowImage);
            DisplayDescription = new RelayCommand(ShowDescription, o => Tour != null);
            RefreshImageCommand = new RelayCommand(RefreshImage, CanRefreshImage);

            TourImage = _bitmapImageService.ToBitmapImage(_tourImageService.DefaultImage);
        }

        private void RefreshImage(object? obj)
        {
            if (CanRefreshImage(obj) && Tour != null)
            {
                UpdateMap(Tour, true);
            }
        }
        private bool CanRefreshImage(object? obj)
        {
            return Tour != null && _tourCollectionService.Online;
        }

        private void ShowDescription(object? obj)
        {
            ImageEnabled = Visibility.Hidden;
            DescriptionEnabled = Visibility.Visible;
            OnPropertyChanged(nameof(DescriptionEnabled));
            OnPropertyChanged(nameof(ImageEnabled));
        }

        private void ShowImage(object? obj)
        {
            ImageEnabled = Visibility.Visible;
            DescriptionEnabled = Visibility.Hidden;
            OnPropertyChanged(nameof(DescriptionEnabled));
            OnPropertyChanged(nameof(ImageEnabled));
        }

        public void UpdateMap(Tour tour, bool forceUpdate)
        {
            TourImage = _bitmapImageService.ToBitmapImage(
                _tourImageService.GetTourImage(tour, forceUpdate));
            OnPropertyChanged(nameof(TourImage));
        }
    }
}
