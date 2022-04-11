using TourPlanner.Common.Models;

namespace TourPlanner.Server.BL.Common.Interfaces
{
    public interface ICoordinatesService
    {
        public Task<TourPoint?> GetCoordinates(string address);
    }
}
