using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.UI.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        public ObservableCollection<LogPoint> Data { get; } = new();
        public LogPoint? SelectedItem { get; set; }

        public ICommand AddLogPoint { get; }
        public ICommand RemoveLogPoint { get; }

        public LogViewModel()
        {

            Data.Add(new LogPoint() { Date = DateTime.Now, Duration = 2f, Distance = 5.4f });
            Data.Add(new LogPoint() { Date = new DateTime(2022, 03, 01), Duration = 3.45f, Distance = 12.4f });


            AddLogPoint = new RelayCommand(
                o =>
                {
                    Data.Add(new LogPoint());
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
