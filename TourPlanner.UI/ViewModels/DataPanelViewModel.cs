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
    public class DataPanelViewModel
    {
        public ObservableCollection<TourPoint> Data { get; } = new();

        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }

        public TourPoint? SelectedItem { get; set; }

        public DataPanelViewModel()
        {

            Data.Add(new TourPoint() { Date = DateTime.Now, Duration = 2f, Distance = 5.4f });
            Data.Add(new TourPoint() { Date = new DateTime(2022, 03, 01), Duration = 3.45f, Distance = 12.4f });


            AddRowCommand = new RelayCommand(
                o =>
                {
                    Data.Add(new TourPoint());
                },
                o => true);
            RemoveRowCommand = new RelayCommand(
                o =>
                {
                    if(SelectedItem != null)
                        Data.Remove(SelectedItem);
                },
                o => true);
        }
    }
}
