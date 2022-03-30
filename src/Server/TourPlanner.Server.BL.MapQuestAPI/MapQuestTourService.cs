using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;
using TourPlanner.Server.BL.Common.Interfaces;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public class MapQuestTourService : MapQuestBase, ITourService
    {
        public MapQuestTourService(string apiKey) : base(apiKey) { }

        public async Task<float> GetDistance(TourPoint start, TourPoint end)
        {
            Uri uri = new($"https://www.mapquestapi.com/directions/v2/route?key={_apiKey}&from={start.Latitude.ToString().Replace(",", ".")}, {start.Longitude.ToString().ToString().Replace(",", ".")}&to={end.Latitude.ToString().Replace(",", ".")}, {end.Longitude.ToString().Replace(",", ".")}");
            string content = await(await _httpClient.GetAsync(uri)).Content.ReadAsStringAsync();
            var jsonData = (JObject?)JsonConvert.DeserializeObject(content);

            if (jsonData == null)
                return -1;

            // Parse distance
            float distance = float.Parse(jsonData["route"]?["distance"]?.ToString() ?? "");

            return distance;
        }
    }
}
