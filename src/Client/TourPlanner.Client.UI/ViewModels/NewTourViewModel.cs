using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class NewTourViewModel : BaseViewModel
    {
        private readonly ITourCollectionService _tourCollectionService;
        private readonly IApiService _apiService;
        private readonly ITourImageService _tourImageService;
        private readonly IBitmapImageService _bitmapImageService;
        public BitmapImage? StartImagePath { get; set; }
        public BitmapImage? EndImagePath { get; set; }

        public Tour? NewTour;
        public TourPoint? StartTourPoint;
        public TourPoint? EndTourPoint;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? StartRoad { get; set; }
        public string? StartNumber { get; set; }
        public string? StartZip { get; set; }
        public string? StartCountry { get; set; }
        public string? StartLatitude { get; set; }
        public string? StartLongitude { get; set; }
        public string? EndRoad { get; set; }
        public string? EndNumber { get; set; }
        public string? EndZip { get; set; }
        public string? EndCountry { get; set; }
        public string? EndLatitude { get; set; }
        public string? EndLongitude { get; set; }

        public Dictionary<Common.Models.TransportType, string> TransportTypes { get; } = new()
        {
            { Common.Models.TransportType.AUTO, "Car" },
            { Common.Models.TransportType.WALKING, "On foot" },
            { Common.Models.TransportType.BICYCLE, "Bicycle" }
        };

        private Common.Models.TransportType _transportType;
        public Common.Models.TransportType TransportType
        {
            get { return _transportType; }
            set
            {
                _transportType = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateTour { get; }
        public ICommand TranslateStartAddress { get; }
        public ICommand TranslateEndAddress { get; }

        public NewTourViewModel(
            ITourCollectionService tourCollectionService,
            IApiService apiService,
            ITourImageService tourImageService,
            IBitmapImageService bitmapImageService)
        {
            _tourCollectionService = tourCollectionService;
            _apiService = apiService;
            _tourImageService = tourImageService;
            _bitmapImageService = bitmapImageService;

            StartImagePath = _bitmapImageService.ToBitmapImage(tourImageService.DefaultImage);
            EndImagePath = _bitmapImageService.ToBitmapImage(tourImageService.DefaultImage);

            CreateTour = new RelayCommand(
                o =>
                {
                    // Check for name and coordinates
                    if (NecessaryInputProvided())
                    {
                        // Create new tour
                        Random rnd = new();
                        var tour = new Tour()
                        {
                            Id = rnd.Next(),
                            Name = Name,
                            EndPoint = new()
                            {
                                Latitude = float.Parse(EndLatitude),
                                Longitude = float.Parse(EndLongitude),
                            },
                            StartPoint = new()
                            {
                                Latitude = float.Parse(StartLatitude),
                                Longitude = float.Parse(StartLongitude),
                            },
                            Description = Description ?? "",
                            Entries = new(),
                            TransportType = TransportType,
                        };
                        if (_tourCollectionService.CreateTourApi(ref tour))
                            _tourCollectionService.AllTours.Add(tour);

                        // Close window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is NewTourWindow)
                            {
                                window.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fill out all required fields", "Create error", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                },
                o => true);
            TranslateStartAddress = new RelayCommand(
                o =>
                {
                    GetStartCoordinatesFromAddress();
                    UpdateStartImage();
                },
                o => true);
            TranslateEndAddress = new RelayCommand(
                o =>
                {
                    GetEndCoordinatesFromAddress();
                    UpdateEndImage();
                },
                o => true);
        }

        private bool UpdateStartImage()
        {
            if (StartTourPoint != null)
            {
                StartImagePath = _bitmapImageService.ToBitmapImage(
                    _tourImageService.GetTourPointImage(StartTourPoint));
                OnPropertyChanged(nameof(StartImagePath));
                return true;
            }
            return false;
        }
        private bool UpdateEndImage()
        {
            if (EndTourPoint != null)
            {
                EndImagePath = _bitmapImageService.ToBitmapImage(
                    _tourImageService.GetTourPointImage(EndTourPoint));
                OnPropertyChanged(nameof(EndImagePath));
                return true;
            }
            return false;
        }

        private bool GetStartCoordinatesFromAddress()
        {
            // Get coordinates from api
            var result = _apiService.GetStringAsync($"Coordinates/?address={StartRoad},{StartNumber},{StartZip},{StartCountry}").Result;

            if (result.Item2 != HttpStatusCode.OK)
                return false;

            // Update Start Tourpoint
            StartTourPoint = JsonConvert.DeserializeObject<TourPoint>(result.Item1);

            // Update View
            StartLatitude = StartTourPoint?.Latitude.ToString();
            StartLongitude = StartTourPoint?.Longitude.ToString();
            OnPropertyChanged(nameof(StartLatitude));
            OnPropertyChanged(nameof(StartLongitude));

            return true;
        }

        private bool GetEndCoordinatesFromAddress()
        {
            // Get coordinates from api
            var result = _apiService.GetStringAsync($"Coordinates/?address={EndRoad},{EndNumber},{EndZip},{EndCountry}").Result;

            if (result.Item2 != HttpStatusCode.OK)
                return false;

            // Update Start Tourpoint
            EndTourPoint = JsonConvert.DeserializeObject<TourPoint>(result.Item1);

            // Update View
            EndLatitude = EndTourPoint?.Latitude.ToString();
            EndLongitude = EndTourPoint?.Longitude.ToString();
            OnPropertyChanged(nameof(EndLatitude));
            OnPropertyChanged(nameof(EndLongitude));

            return true;
        }

        private bool NecessaryInputProvided()
        {
            if (Name == null || Name.Length == 0) return false;
            if (StartLatitude == null || StartLatitude.Length == 0) return false;
            if (StartLongitude == null || StartLongitude.Length == 0) return false;
            if (EndLatitude == null || EndLatitude.Length == 0) return false;
            if (EndLongitude == null || EndLongitude.Length == 0) return false;
            return true;
        }
    }
}
