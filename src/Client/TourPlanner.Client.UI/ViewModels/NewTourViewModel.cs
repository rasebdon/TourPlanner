using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public BitmapImage StartImagePath { get; set; }
        public BitmapImage EndImagePath { get; set; }

        public Tour? NewTour;
        public TourPoint? StartTourPoint;
        public TourPoint? EndTourPoint;
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartRoad { get; set; }
        public string StartNumber { get; set; }
        public string StartZip { get; set; }
        public string StartCountry { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndRoad { get; set; }
        public string EndNumber { get; set; }
        public string EndZip { get; set; }
        public string EndCountry { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }

        public ICommand CreateTour { get; }
        public ICommand TranslateStartAddress { get; }
        public ICommand TranslateEndAddress { get; }

        public NewTourViewModel(
            ITourCollectionService tourCollectionService,
            IApiService apiService)
        {
            _tourCollectionService = tourCollectionService;
            _apiService = apiService;

            EndImagePath = GetImageFromPath("assets/images/no_image.jpg");
            StartImagePath = GetImageFromPath("assets/images/no_image.jpg");

            Name = "";
            Description = "";
            StartRoad = "";
            StartNumber = "";
            StartZip = "";
            StartCountry = "";
            StartLatitude = "";
            StartLongitude = "";
            EndRoad = "";
            EndNumber = "";
            EndZip = "";
            EndCountry = "";
            EndLatitude = "";
            EndLongitude = "";

            CreateTour = new RelayCommand(
                o =>
                {
                    // Check for name and coordinates
                    if (NecessaryInputProvided())
                    {
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
                            TransportType = Common.Models.TransportType.AUTO,
                            Entries = new()
                        };
                        if (_tourCollectionService.SaveTourApi(ref tour))
                            _tourCollectionService.AllTours.Add(tour);
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
            // Get image from api
            var result = _apiService.GetBytesAsync($"Coordinates/Map?lat={StartLatitude}&lon={StartLongitude}").Result;

            if (result.Item2 != HttpStatusCode.OK)
                return false;

            StartImagePath = ToBitmapImage(result.Item1);            
            OnPropertyChanged(nameof(StartImagePath));
            return true;
        }
        private bool UpdateEndImage()
        {
            // Get image from api
            var result = _apiService.GetBytesAsync($"Coordinates/Map?lat={EndLatitude}&lon={EndLongitude}").Result;

            if (result.Item2 != HttpStatusCode.OK)
                return false;

            EndImagePath = ToBitmapImage(result.Item1);
            OnPropertyChanged(nameof(EndImagePath));
            return true;
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
            StartLatitude = StartTourPoint.Latitude.ToString();
            StartLongitude = StartTourPoint.Longitude.ToString();
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
            EndLatitude = EndTourPoint.Latitude.ToString();
            EndLongitude = EndTourPoint.Longitude.ToString();
            OnPropertyChanged(nameof(EndLatitude));
            OnPropertyChanged(nameof(EndLongitude));

            return true;
        }


        private static BitmapImage GetImageFromPath(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }

        public static BitmapImage ToBitmapImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;//CacheOption must be set after BeginInit()
                img.StreamSource = ms;
                img.EndInit();

                if (img.CanFreeze)
                {
                    img.Freeze();
                }


                return img;
            }
        }

        private bool NecessaryInputProvided()
        {
            if (Name.Length == 0) return false;
            if (StartLatitude.Length == 0) return false;
            if (StartLongitude.Length == 0) return false;
            if (EndLatitude.Length == 0) return false;
            if (EndLongitude.Length == 0) return false;
            return true;
        }

    }
}
