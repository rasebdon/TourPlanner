using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TourPlanner.Client.UI.Services;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class NewTourViewModel : BaseViewModel
    {
        private readonly ITourCollectionService _tourCollectionService;
        private readonly IApiService _apiService;
        public BitmapImage StartImagePath { get; set; }
        public BitmapImage EndImagePath { get; set; }

        public Tour? NewTour;

        public string Name { get; set; }
        public string Description { get; set; }
        public string StartRoad { get; set; }
        public string StartNumber { get; set; }
        public string StartZip { get; set; }
        public string StartLattitude { get; set; }
        public string StartLongtitude { get; set; }
        public string EndRoad { get; set; }
        public string EndNumber { get; set; }
        public string EndZip { get; set; }
        public string EndLattitude { get; set; }
        public string EndLongtitude { get; set; }

        public ICommand CreateTour { get; }
        //public ICommand TranslateStartAddress { get; }
        //public ICommand TranslateEndAddress { get; }

        public NewTourViewModel(
            ITourCollectionService tourCollectionService,
            IApiService apiService)
        {
            _tourCollectionService = tourCollectionService;
            _apiService = apiService;

            EndImagePath = GetImageFromPath("assets/images/no_image.jpg");
            StartImagePath = GetImageFromPath("assets/images/no_image.jpg");

            Name = "";
            Description = "";
            StartRoad = "";
            StartNumber = "";
            StartZip = "";
            StartLattitude = "";
            StartLongtitude = "";
            EndRoad = "";
            EndNumber = "";
            EndZip = "";
            EndLattitude = "";
            EndLongtitude = "";

            CreateTour = new RelayCommand(
                o =>
                {
                    MessageBox.Show(Name);
                },
                o => true);
        }

        private static BitmapImage GetImageFromPath(string path)
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }


    }
}
