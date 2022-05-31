﻿using Newtonsoft.Json;
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

        public Tour? SelectedTour { get; set; }
        public TourEntry? SelectedTourEntry { get; set; }

        public ICommand AddLogEntryCommand { get; }
        public ICommand RemoveLogEntryCommand { get; }
        public ICommand SaveTableCommand { get; }

        public bool IsReadOnly
        {
            get
            {
                return !_tourCollectionService.Online;
            }
        }

        private readonly IApiService _apiService;
        private readonly ITourCollectionService _tourCollectionService;
        private readonly ITourSelectionService _tourSelectionService;

        public LogViewModel(
            IApiService apiService,
            ITourCollectionService tourCollectionService,
            ITourSelectionService tourSelectionService)
        {
            _apiService = apiService;
            _tourCollectionService = tourCollectionService;
            _tourSelectionService = tourSelectionService;

            _tourSelectionService.OnTourChanged += OnTourChanged;

            AddLogEntryCommand = new RelayCommand(
                AddLogEntry,
                o => SelectedTour != null && _tourCollectionService.Online);

            RemoveLogEntryCommand = new RelayCommand(
                RemoveLogEntry,
                o => SelectedTourEntry != null && _tourCollectionService.Online);

            SaveTableCommand = new RelayCommand(
                o =>
                {
                    var entry = SelectedTourEntry;

                    OnPropertyChanged(nameof(SelectedTourEntry));

                    if (entry != null && _tourCollectionService.Online && !UpdateTourEntry(ref entry))
                        MessageBox.Show(
                           "No connection to server",
                           "An error occured while updating the table!",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
                },
                o => true);
        }

        private void OnTourChanged(object? sender, TourChangedEventArgs e)
        {
            Data.Clear();
            SelectedTour = e.NewValue;
            if (SelectedTour != null)
            {
                SelectedTour.Entries.ForEach(e => Data.Add(e));
            }
            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(SelectedTour));
        }

        private void RemoveLogEntry(object? obj)
        {
            if (SelectedTourEntry != null && DeleteTourEntry(SelectedTourEntry.Id))
            {
                Data.Remove(SelectedTourEntry);
                SelectedTour?.Entries.Remove(SelectedTourEntry);
                SelectedTourEntry = null;
            }
        }

        private void AddLogEntry(object? obj)
        {
            if (SelectedTour == null)
                return;

            var entry = new TourEntry() { Date = DateTime.Now, Id = -1, TourId = SelectedTour.Id };
            // Create entry
            if (CreateTourEntry(ref entry))
            {
                Data.Add(entry);
                SelectedTour.Entries.Add(entry);
            }
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
