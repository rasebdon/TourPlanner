﻿using TourPlanner.Common.Models;

namespace TourPlanner.Server.BL.Common.Interfaces
{
    public interface IMapService
    {
        public Task<IEnumerable<byte>> GetRouteMap(TourPoint start, TourPoint end, float width, float height);
        public Task<IEnumerable<byte>> GetLocationMap(float lat, float lon);
    }
}
