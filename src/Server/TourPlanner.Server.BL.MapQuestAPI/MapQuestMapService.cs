using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestMapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MapQuestMapService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<byte[]> GetRouteMap(TourPoint start, TourPoint end)
        {
            Uri uri = new($"https://www.mapquestapi.com/staticmap/v5/map?key={_apiKey}&start={start.Latitude.ToString().Replace(",", ".")}, {start.Longitude.ToString().ToString().Replace(",", ".")}&end={end.Latitude.ToString().Replace(",", ".")}, {end.Longitude.ToString().Replace(",", ".")}");
            return await (await _httpClient.GetAsync(uri)).Content.ReadAsByteArrayAsync();
        }
    }
}