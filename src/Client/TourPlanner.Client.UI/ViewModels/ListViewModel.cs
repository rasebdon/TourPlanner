using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.ViewModels
{
    public class ListViewModel : BaseViewModel
    {
        public ObservableCollection<Tour> Data { get; } = new();
        public Tour? SelectedItem { get; set; }

        public ICommand AddListPoint { get; }
        public ICommand RemoveListPoint { get; }

        public ListViewModel()
        {
            Data.Add(new Tour() { Name = "Tour 1" });
            Data.Add(new Tour() { Name = "Tour 2" });
            Data.Add(new Tour() { Name = "Tour 3" });

            AddListPoint = new RelayCommand(
                o =>
                {
                    Data.Add(new Tour() { Name = "New Tour"});
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
