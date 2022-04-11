using System;
using System.Collections.ObjectModel;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public interface ITourCollectionService
    {
        public bool Online { get; }

        public ObservableCollection<Tour> AllTours { get; }
        public ObservableCollection<Tour> DisplayedTours { get; }

        public bool CreateTourApi(ref Tour tour);
        public bool UpdateTourApi(ref Tour tour);
        public bool DeleteTourApi(int tourId);
        public bool LoadToursApi();

        public void Export(Uri filePath);
        public void Import(Uri filePath);
    }
}
