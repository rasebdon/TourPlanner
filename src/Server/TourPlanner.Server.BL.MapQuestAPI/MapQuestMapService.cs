using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestMapService : MapQuestBase, IMapService
    {
        public MapQuestMapService(string apiKey) : base(apiKey) { }

        public async Task<IEnumerable<byte>> GetRouteMap(TourPoint start, TourPoint end)
        {
            Uri uri = new($"https://www.mapquestapi.com/staticmap/v5/map?key={_apiKey}&start={start.Latitude.ToString().Replace(",", ".")}, {start.Longitude.ToString().ToString().Replace(",", ".")}&end={end.Latitude.ToString().Replace(",", ".")}, {end.Longitude.ToString().Replace(",", ".")}");
            var content = await (await _httpClient.GetAsync(uri)).Content.ReadAsByteArrayAsync();
            return content;
        }
    }
}