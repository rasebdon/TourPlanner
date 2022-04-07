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
        public ObservableCollection<TourEntry> Data { get; private set; } = new();

        private Tour? _tour;
        public Tour? Tour 
        {
            get
            {
                return _tour;
            }
            set
            {
                _tour = value;
                if(_tour != null)
                {
                    Data.Clear();
                    _tour.Entries.ForEach(e => Data.Add(e));
                }
            }
        }
        public TourEntry? SelectedItem { get; set; }

        public ICommand AddLogPoint { get; }
        public ICommand RemoveLogPoint { get; }

        public LogViewModel()
        {
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
