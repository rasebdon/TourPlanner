using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public class TourCollectionService : ITourCollectionService
    {
        public ICollection<Tour> Tours { get; private set; } = new List<Tour>();

        private readonly IApiService _apiService;

        public TourCollectionService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public bool Export(Uri filePath)
        {
            throw new NotImplementedException();
        }

        public bool Import(Uri filePath)
        {
            throw new NotImplementedException();
        }

        public bool LoadToursApi()
        {
            // Get tour collection string from api
            var response = _apiService.GetStringAsync("Tour").Result;

            if (response.Item2 != System.Net.HttpStatusCode.OK)
                return false;

            // Parse string as collection
            var tours = JsonConvert.DeserializeObject<List<Tour>>(response.Item1);
            if (tours == null)
                return false;

            Tours = tours;
            return true;
        }

        public bool SaveTourApi(ref Tour tour)
        {
            // Check if tour already exists via id
            (string, HttpStatusCode) response;
            if(tour.Id != -1)
            {
                // Post
                response = _apiService.PostAsync("Tour", tour).Result;
            }
            else
            {
                // Put
                response = _apiService.PutAsync($"Tour/{tour.Id}", tour).Result;
            }

            // Check if saving was successful
            if (response.Item2 != HttpStatusCode.OK)
                return false;

            // Parse string as tour
            var returnedTour = JsonConvert.DeserializeObject<Tour>(response.Item1);
            if (returnedTour == null)
                return false;

            tour = returnedTour;

            return true;
        }
    }
}
