using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public interface ITourImageService
    {
        byte[] GetTourImage(Tour tour, bool update);
        byte[] GetTourPointImage(TourPoint tour);
    }
}
