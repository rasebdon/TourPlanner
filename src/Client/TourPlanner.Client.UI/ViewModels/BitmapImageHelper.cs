using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TourPlanner.Client.UI.ViewModels
{
    public static class BitmapImageHelper
    {
        public static BitmapImage GetImageFromPath(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }

        public static byte[] GetImageBytesFromPath(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return File.ReadAllBytes(path);
        }

        public static BitmapImage ToBitmapImage(byte[] data)
        {
            using MemoryStream ms = new(data);

            BitmapImage img = new();
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
}
