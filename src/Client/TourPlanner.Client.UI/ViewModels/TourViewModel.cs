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
        public BitmapImage ImagePath { get; set; }

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

                    Description = _tour.Description;
                }
                else
                {
                    Description = "";
                    ImagePath = GetImageFromPath("assets/images/no_image.jpg");
                }
                OnPropertyChanged(nameof(Tour));
                OnPropertyChanged(nameof(ImagePath));
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Description { get; set; } = "";

        public Visibility ImageEnabled { get; set; } = Visibility.Visible;
        public Visibility DescriptionEnabled { get; set; } = Visibility.Hidden;

        public ICommand DisplayRoute { get; }
        public ICommand DisplayDescription { get; }


        private readonly IApiService _apiService;
        private readonly ITourCollectionService _tourCollectionService;

        public TourViewModel(
            IApiService apiService,
            ITourCollectionService tourCollectionService)
        {
            _apiService = apiService;
            _tourCollectionService = tourCollectionService;

            DisplayRoute = new RelayCommand(ShowImage);
            DisplayDescription = new RelayCommand(ShowDescription);

            ImagePath = GetImageFromPath("assets/images/no_image.jpg");

            Directory.CreateDirectory("assets");
            Directory.CreateDirectory("assets/images");
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

        public bool UpdateMap(Tour tour, bool forceUpdate)
        {
            // Check if image is already saved
            var path = $"assets/images/{tour.Id}.jpg";
            if (forceUpdate || !File.Exists(path))
            {
                // Get image from api
                var result = _apiService.GetBytesAsync($"Tour/{tour.Id}/Map").Result;

                if (result.Item2 != HttpStatusCode.OK)
                    return false;

                // Override image
                File.WriteAllBytes(path, result.Item1);
            }

            ImagePath = GetImageFromPath(path);

            return true;
        }

        private static BitmapImage GetImageFromPath(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }
    }
}
