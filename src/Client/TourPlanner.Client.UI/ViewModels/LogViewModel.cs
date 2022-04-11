using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Client.UI.Services;
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
                Data.Clear();
                if (_tour != null)
                {
                    _tour.Entries.ForEach(e => Data.Add(e));
                }
                OnPropertyChanged(nameof(Data));
                OnPropertyChanged(nameof(Tour));
            }
        }
        public TourEntry? SelectedItem { get; set; }

        public ICommand AddLogPoint { get; }
        public ICommand RemoveLogPoint { get; }
        public ICommand SaveTableCommand { get; }

        private readonly IApiService _apiService;
        private readonly ITourCollectionService _tourCollectionService;

        public LogViewModel(
            IApiService apiService,
            ITourCollectionService tourCollectionService)
        {
            _apiService = apiService;
            _tourCollectionService = tourCollectionService;

            AddLogPoint = new RelayCommand(
                o =>
                {
                    if (Tour == null)
                        return;

                    var entry = new TourEntry() { Date = DateTime.Now, Id = -1, TourId = Tour.Id };
                    // Create entry
                    if (CreateTourEntry(ref entry))
                    {
                        Data.Add(entry);
                    }
                },
                o => true);

            RemoveLogPoint = new RelayCommand(
                o =>
                {
                    if (SelectedItem != null && DeleteTourEntry(SelectedItem.Id))
                    {
                        Data.Remove(SelectedItem);
                        SelectedItem = null;
                    }
                },
                o => true);

            SaveTableCommand = new RelayCommand(
                o =>
                {
                    var entry = SelectedItem;

                    OnPropertyChanged(nameof(SelectedItem));

                    if (entry != null && _tourCollectionService.Online && !UpdateTourEntry(ref entry))
                        MessageBox.Show(
                           "No connection to server",
                           "An error occured while updating the table!",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
                },
                o => true);
        }

        private bool CreateTourEntry(ref TourEntry entry)
        {
            try
            {
                // Send post request
                (string, HttpStatusCode) response = _apiService.PostAsync(
                    $"Tour/{entry.TourId}/Entry", entry).Result;

                // Check if post was successful
                if (response.Item2 != HttpStatusCode.Created)
                    return false;

                // Parse string as tour entry
                var returnedEntry = JsonConvert.DeserializeObject<TourEntry>(response.Item1);
                if (returnedEntry == null)
                    return false;

                entry = returnedEntry;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool UpdateTourEntry(ref TourEntry entry)
        {
            try
            {
                // Send put request
                (string, HttpStatusCode) response = _apiService.PutAsync(
                    $"Tour/Entry/{entry.Id}", entry).Result;

                // Check if saving was successful
                if (response.Item2 != HttpStatusCode.OK)
                    return false;

                // Parse string as tour entry
                var returnedEntry = JsonConvert.DeserializeObject<TourEntry>(response.Item1);
                if (returnedEntry == null)
                    return false;

                entry = returnedEntry;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteTourEntry(int id)
        {
            try
            {
                // Send delete request
                HttpStatusCode response = _apiService.DeleteAsync(
                    $"Tour/Entry/{id}").Result;

                // Check if saving was successful
                return response == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }
    }
}
