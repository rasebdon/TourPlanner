using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Models;

namespace TourPlanner.Server.BL.Common.Interfaces
{
    public interface IRouteService
    {
        Task<RouteInfo?> GetRouteInfo(TourPoint start, TourPoint end, TransportType transportType);
    }
}
