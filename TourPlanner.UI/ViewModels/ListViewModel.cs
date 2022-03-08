using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace TourPlanner.UI.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        public ObservableCollection<ListPoint> Data { get; } = new();
        public ListPoint? SelectedItem { get; set; }

        public ICommand AddListPoint { get; }
        public ICommand RemoveListPoint { get; }

        public ListViewModel()
        {
            Data.Add(new ListPoint() { Name = "Tour 1" });
            Data.Add(new ListPoint() { Name = "Tour 2" });
            Data.Add(new ListPoint() { Name = "Tour 3" });

            AddListPoint = new RelayCommand(
                o =>
                {
                    Data.Add(new ListPoint() { Name = "New Tour"});
                },
                o => true);
            RemoveListPoint = new RelayCommand(
                o =>
                {
                    if (SelectedItem != null)
                        Data.Remove(SelectedItem);
                },
                o => true);
        }
    }
}
