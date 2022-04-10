using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;
using TourPlanner.Server.BL.Common.Models;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestTourService : MapQuestBase, IRouteService
    {
        public MapQuestTourService(string apiKey) : base(apiKey) { }

        public async Task<RouteInfo?> GetRouteInfo(TourPoint start, TourPoint end, TransportType transportType)
        {
            string routeType = transportType switch
            {
                TransportType.AUTO => "fastest",
                TransportType.WALKING => "pedestrian",
                TransportType.BICYCLE => "bicycle",
                _ => "fastest",
            };

            Uri uri = new(
                $"https://www.mapquestapi.com/directions/v2/route?key={_apiKey}" +
                $"&from={start.Latitude.ToString().Replace(",", ".")}, {start.Longitude.ToString().ToString().Replace(",", ".")}" +
                $"&to={end.Latitude.ToString().Replace(",", ".")}, {end.Longitude.ToString().Replace(",", ".")}&unit=k" +
                $"&routeType={routeType}");
            string content = await(await _httpClient.GetAsync(uri)).Content.ReadAsStringAsync();
            var jsonData = (JObject?)JsonConvert.DeserializeObject(content);

            if (jsonData == null)
                return null;

            // Check for errors
            var statusCode = int.Parse(jsonData["info"]?["statuscode"]?.ToString() ?? "-1");
            if (statusCode != 0)
                throw new NoPathException();

            // Parse distance
            return new()
            {
                Distance = float.Parse(jsonData["route"]?["distance"]?.ToString() ?? ""),
                Time = int.Parse(jsonData["route"]?["time"]?.ToString() ?? "")
            };
        }
    }
}
