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

namespace TourPlanner.Client.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ListViewModel ListViewModel { get; }
        //public TourViewModel TourViewModel { get; }
        public LogViewModel LogViewModel { get; }

        public MainViewModel()
        {
            ListViewModel = new();
            //TourViewModel = new();
            LogViewModel = new();
        }
    }
}
