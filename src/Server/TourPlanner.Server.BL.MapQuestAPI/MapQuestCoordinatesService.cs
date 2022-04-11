using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestCoordinatesService : MapQuestBase, ICoordinatesService
    {
        public MapQuestCoordinatesService(string apiKey) : base(apiKey) { }
        public async Task<TourPoint?> GetCoordinates(string address)
        {
            Uri uri = new($"https://www.mapquestapi.com/geocoding/v1/address?key={_apiKey}&location={address}");

            string content = await (await _httpClient.GetAsync(uri)).Content.ReadAsStringAsync();
            var jsonData = (JObject?)JsonConvert.DeserializeObject(content);

            if (jsonData == null)
                return null;

            // Parse distance
            return new()
            {
                Latitude = float.Parse(jsonData["results"]?.FirstOrDefault()?["locations"]?.FirstOrDefault()?["latLng"]?["lat"]?.ToString() ?? ""),
                Longitude = float.Parse(jsonData["results"]?.FirstOrDefault()?["locations"]?.FirstOrDefault()?["latLng"]?["lng"]?.ToString() ?? "")
            };
        }
    }
}