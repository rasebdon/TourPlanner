using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public class TourSelectionService : ITourSelectionService
    {
        private Tour? _tour;
        public Tour? Tour 
        {
            get
            {
                return _tour;
            } 
            set 
            {
                Tour? previousTour = _tour;
                _tour = value;

                OnTourChanged?.Invoke(
                    this,
                    new TourChangedEventArgs()
                    {
                        PreviousValue = previousTour,
                        NewValue = value
                    });
            }
        }

        public event EventHandler<TourChangedEventArgs>? OnTourChanged;
    }
}
