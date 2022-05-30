using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TourPlanner.Client.UI.Services
{
    public interface IBitmapImageService
    {
        public BitmapImage GetImageFromPath(string path);
        public byte[] GetImageBytesFromPath(string path);
        public BitmapImage ToBitmapImage(byte[] data);
    }
}
