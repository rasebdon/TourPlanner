using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Server.BL.MapQuestAPI
{
    public abstract class MapQuestBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _apiKey;

        protected MapQuestBase(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }
    }
}
