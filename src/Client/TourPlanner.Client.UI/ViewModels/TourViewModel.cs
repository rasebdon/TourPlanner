using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                if(_tour != null)
                {
                    UpdateMap(_tour);
                }
                else
                {
                    ImagePath = GetImageFromPath("assets/images/no_image.jpg");
                }
            }
        }

        private readonly IApiService _apiService;

        public TourViewModel(IApiService apiService)
        {
            ImagePath = GetImageFromPath("assets/images/no_image.jpg");

            _apiService = apiService;

            Directory.CreateDirectory("assets");
            Directory.CreateDirectory("assets/images");
        }

        public bool UpdateMap(Tour tour)
        {
            // Check if image is already saved
            var path = $"assets/images/{tour.Id}.jpg";
            if (!File.Exists(path))
            {
                // Get image from api
                var result = _apiService.GetBytesAsync($"Tour/{tour.Id}/Map").Result;

                if (result.Item2 != HttpStatusCode.OK)
                    return false;

                // Override image
                File.WriteAllBytes(path, result.Item1);
            }

            ImagePath = GetImageFromPath(path);
            OnPropertyChanged(nameof(ImagePath));

            return true;
        }

        private static BitmapImage GetImageFromPath(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }
    }
}
