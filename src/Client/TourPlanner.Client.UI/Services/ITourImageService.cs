using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public interface ITourImageService
    {
        byte[] DefaultImage { get; }
        byte[] GetTourImage(Tour tour, bool update);
        byte[] GetTourPointImage(TourPoint tourPoint);
    }
}
