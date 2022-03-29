using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public interface IMapService
    {
        public Task<byte[]> GetRouteMap(string start, string end);
    }
}
