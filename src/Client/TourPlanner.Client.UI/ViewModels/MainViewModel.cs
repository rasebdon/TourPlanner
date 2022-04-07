using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITourCollectionService _tourCollectionService;

        public MainViewModel(ITourCollectionService tourCollectionService)
        {
            _tourCollectionService = tourCollectionService;

            // Get tours from API and close application if it cannot be retrieved
            if (!_tourCollectionService.LoadToursApi())
            {
                Application.Current.Shutdown();
            }
        }
    }
}
