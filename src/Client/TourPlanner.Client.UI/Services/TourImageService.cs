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
        private BitmapImage _noImageBitmapImage;

        public TourImageService(IApiService apiService)
        {
            _apiService = apiService;
            _tourImages = new();
            _noImageBitmapImage = BitmapImageHelper.ToBitmapImage(
                BitmapImageHelper.GetImageBytesFromPath("assets/images/no_image.jpg"));
        }

        public byte[] GetTourImage(Tour tour, bool update)
        {
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
            if(_tourImages.TryGetValue(tour.Id, out var image))
            {
                return image;
            }
            else
            {
                return BitmapImageHelper.GetImageBytesFromPath(path);
            }
        }

        public byte[] GetTourPointImage(TourPoint tour)
        {
            throw new NotImplementedException();
        }
    }
}
