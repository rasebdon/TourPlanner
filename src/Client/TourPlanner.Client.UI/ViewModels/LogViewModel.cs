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
                    Duration = 7200,
                    Distance = 5.4f,
                });
            Data.Add(
                new TourEntry()
                {
                    Date = new DateTime(2022, 03, 01),
                    Duration = 11345,
                    Distance = 12.4f,
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
