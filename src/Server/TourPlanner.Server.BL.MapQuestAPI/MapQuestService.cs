using Newtonsoft.Json;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MapQuestService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<byte[]> GetRouteMap(string start, string end)
        {
            Uri uri = new($"https://www.mapquestapi.com/staticmap/v5/map?key={_apiKey}&start={start}&end={end}");
            return await (await _httpClient.GetAsync(uri)).Content.ReadAsByteArrayAsync();
        }
    }
}