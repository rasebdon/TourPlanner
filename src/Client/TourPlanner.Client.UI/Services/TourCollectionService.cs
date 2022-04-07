using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public class TourCollectionService : ITourCollectionService
    {
        public ObservableCollection<Tour> Tours { get; private set; } = new ObservableCollection<Tour>();

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
            try
            {
                // Get tour collection string from api
                var response = _apiService.GetStringAsync("Tour").Result;

                if (response.Item2 != HttpStatusCode.OK)
                    return false;

                // Parse string as collection
                var tours = JsonConvert.DeserializeObject<List<Tour>>(response.Item1);
                if (tours == null)
                    return false;

                Tours.Clear();
                Tours = new ObservableCollection<Tour>(tours);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Error retrieving tours from server!\nMaybe check your internet connection?",
                    "Could not get tours!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            }
            return false;
        }

        public bool SaveTourApi(ref Tour tour)
        {
            // Check if tour already exists via id
            (string, HttpStatusCode) response;
            if(tour.Id == -1)
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
            if (response.Item2 != HttpStatusCode.OK || response.Item2 != HttpStatusCode.Created)
                return false;

            // Parse string as tour
            var returnedTour = JsonConvert.DeserializeObject<Tour>(response.Item1);
            if (returnedTour == null)
                return false;

            tour = returnedTour;

            return true;
        }

        public bool DeleteTourApi(int tourId)
        {
            var response = _apiService.DeleteAsync($"Tour/{tourId}").Result;

            return response == HttpStatusCode.OK;
        }
    }
}
