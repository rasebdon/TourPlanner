using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public interface ITourSelectionService
    {
        public Tour? Tour { get; set; }
        public event EventHandler<TourChangedEventArgs>? OnTourChanged;
    }

    public class TourChangedEventArgs : EventArgs
    {
        public Tour? PreviousValue { get; set; }
        public Tour? NewValue { get; set; }
    }
}
