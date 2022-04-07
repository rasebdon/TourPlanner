using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public interface ITourCollectionService
    {
        public ICollection<Tour> Tours { get; }

        public bool SaveTourApi(ref Tour tour);
        public bool LoadToursApi();

        public bool Export(Uri filePath);
        public bool Import(Uri filePath);
    }
}
