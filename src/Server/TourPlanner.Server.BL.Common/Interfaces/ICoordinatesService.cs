using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Server.BL.Common.Interfaces
{
    public interface ICoordinatesService
    {
        public Task<TourPoint?> GetCoordinates(string address);
    }
}
