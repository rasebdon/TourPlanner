using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TourPlanner.Client.UI.ViewModels;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public class TourImageService : ITourImageService
    {
        private readonly Dictionary<int, byte[]> _tourImages;
        private readonly IApiService _apiService;
        private readonly IBitmapImageService _bitmapImageService;
        public byte[] DefaultImage { get; }

        public TourImageService(IApiService apiService, IBitmapImageService bitmapImageService)
        {
            _apiService = apiService;
            _bitmapImageService = bitmapImageService;
            _tourImages = new();
            DefaultImage = _bitmapImageService.GetImageBytesFromPath("assets/images/no_image.jpg");
        }

        public byte[] GetTourImage(Tour tour, bool update)
        {
            if(!Directory.Exists("assets"))
                Directory.CreateDirectory("assets");
            if(!Directory.Exists("assets/images"))
                Directory.CreateDirectory("assets/images");

            // Check if image is already saved
            var path = $"assets/images/{tour.Id}.jpg";
            if (update || !File.Exists(path))
            {
                // Get image from api
                var result = _apiService.GetBytesAsync($"Tour/{tour.Id}/Map?width=1400&height=600").Result;

                if (result.Item2 != HttpStatusCode.OK)
                    return Array.Empty<byte>();

                if (_tourImages.ContainsKey(tour.Id))
                    _tourImages[tour.Id] = result.Item1;
                else
                    _tourImages.Add(tour.Id, result.Item1);

                // Override image
                File.WriteAllBytes(path, result.Item1);
            }

            // Check if the image exists in the dictionary
            if (_tourImages.TryGetValue(tour.Id, out var image))
            {
                return image;
            }
            else
            {
                return _bitmapImageService.GetImageBytesFromPath(path);
            }
        }

        public byte[] GetTourPointImage(TourPoint tourPoint)
        {
            var latString = tourPoint.Latitude.ToString().Replace(',', '.');
            var lonString = tourPoint.Longitude.ToString().Replace(',', '.');

            // Get image from api
            var result = _apiService.GetBytesAsync(
                $"Coordinates/Map?lat={latString}" +
                $"&lon={lonString}")
                .Result;

            if (result.Item2 != HttpStatusCode.OK)
                return DefaultImage;

            return result.Item1;
        }
    }
}
