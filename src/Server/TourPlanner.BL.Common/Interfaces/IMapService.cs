using TourPlanner.Common.Models;

namespace TourPlanner.Server.BL.Common.Interfaces
{
    public interface IMapService
    {
        public Task<byte[]> GetRouteMap(TourPoint start, TourPoint end);
    }
}
