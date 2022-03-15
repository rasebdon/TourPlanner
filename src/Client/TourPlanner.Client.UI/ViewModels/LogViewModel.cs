using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        public ObservableCollection<TourEntry> Data { get; } = new();
        public TourEntry? SelectedItem { get; set; }

        public ICommand AddLogPoint { get; }
        public ICommand RemoveLogPoint { get; }

        public LogViewModel()
        {

            Data.Add(
                new TourEntry()
                { 
                    Date = DateTime.Now,
                    Duration = 2f,
                    Distance = 5.4f,
                    EndPoint = new() { Latitude = 40.02421f, Longitude = 20.51962f },
                    StartPoint = new() { Latitude = 41.02421f, Longitude = 21.51962f },
                });
            Data.Add(
                new TourEntry()
                {
                    Date = new DateTime(2022, 03, 01),
                    Duration = 3.45f,
                    Distance = 12.4f,
                    EndPoint = new() { Latitude = 41.02421f, Longitude = 21.51962f },
                    StartPoint = new() { Latitude = 42.02421f, Longitude = 22.51962f },
                });

            AddLogPoint = new RelayCommand(
                o =>
                {
                    Data.Add(new TourEntry());
                },
                o => true);
            RemoveLogPoint = new RelayCommand(
                o =>
                {
                    if (SelectedItem != null)
                        Data.Remove(SelectedItem);
                },
                o => true);
        }
    }
}
