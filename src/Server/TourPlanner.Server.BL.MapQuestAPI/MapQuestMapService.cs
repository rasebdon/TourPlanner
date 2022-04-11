using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestMapService : MapQuestBase, IMapService
    {
        public MapQuestMapService(string apiKey) : base(apiKey) { }

        public async Task<IEnumerable<byte>> GetRouteMap(TourPoint start, TourPoint end, float width, float height)
        {
            Uri uri = new($"https://www.mapquestapi.com/staticmap/v5/map?key={_apiKey}" +
                $"&start={StrCoord(start.Latitude)}, {StrCoord(start.Longitude)}" +
                $"&end={StrCoord(end.Latitude)}, {StrCoord(end.Longitude)}" +
                ((width != 0 && height != 0) ? $"&size={width}, {height}" : "") +
                $"&type=hyb");
            var content = await (await _httpClient.GetAsync(uri)).Content.ReadAsByteArrayAsync();
            return content;
        }
        public async Task<IEnumerable<byte>> GetLocationMap(float lat, float lon)
        {
            Uri uri = new($"https://www.mapquestapi.com/staticmap/v5/map?key={_apiKey}&center={StrCoord(lat)}, {StrCoord(lon)}&locations={StrCoord(lat)}, {StrCoord(lon)}&zoom=15&size=255,170");
            var content = await (await _httpClient.GetAsync(uri)).Content.ReadAsByteArrayAsync();
            return content;
        }

        private static string StrCoord(float coord)
        {
            return coord.ToString().Replace(",", ".");
        }
    }
}