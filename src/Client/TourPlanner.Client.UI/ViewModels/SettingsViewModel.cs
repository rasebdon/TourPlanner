using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;

namespace TourPlanner.Client.UI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ICommand ClearImageCacheCommand { get; }

        private readonly ITourCollectionService _tourCollectionService;

        public SettingsViewModel(ITourCollectionService tourCollectionService)
        {
            _tourCollectionService = tourCollectionService;

            ClearImageCacheCommand = new RelayCommand(ClearImageCache);
        }

        private void ClearImageCache(object? obj)
        {
            try
            {
                var path = $"assets/images";
                var files = Directory.GetFiles(path);
                int deleted = 0;

                foreach (var file in files)
                {
                    var name = file.Replace('\\', '/').Split("/").Last().Split('.').First();
                    if (int.TryParse(name, out var id) &&
                        !_tourCollectionService.AllTours.Any(t => t.Id == id))
                    {
                        try
                        {
                            File.Delete(file);
                            deleted++;
                        }
                        catch(IOException)
                        {
                            continue;
                        }
                    }
                }

                MessageBox.Show(
                    $"{deleted} images were deleted from the image cache!",
                    "Success!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(
                    "An error occured while clearing the image cache!",
                    "Oh no!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
