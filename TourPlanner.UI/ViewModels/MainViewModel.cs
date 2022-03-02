using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.UI.ViewModels
{
    public class TourPoint
    {
        public DateTime Date { get; set; }
        public float Duration { get; set; }
        public float Distance { get; set; }
    }

    public class MainViewModel : BaseViewModel
    {
        public DataPanelViewModel DataPanelViewModel { get; }

        public MainViewModel()
        {
            DataPanelViewModel = new();
        }
    }
}
