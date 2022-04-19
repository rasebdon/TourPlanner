using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Services
{
    public class TourCollectionService : ITourCollectionService, INotifyPropertyChanged
    {
        // ITourCollectionService
        public bool Online { get; private set; }
        public ObservableCollection<Tour> AllTours { get; private set; } = new ObservableCollection<Tour>();
        public ObservableCollection<Tour> DisplayedTours { get; private set; } = new ObservableCollection<Tour>();

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Dependencies
        private readonly IApiService _apiService;

        public TourCollectionService(IApiService apiService)
        {
            _apiService = apiService;

            AllTours.CollectionChanged += AllTours_CollectionChanged;
        }

        public void PostDummyData()
        {
            var path = $"{Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName}\\assets\\dummy_data\\data.json";
            JObject? obj = (JObject?)JsonConvert.DeserializeObject(File.ReadAllText(path));

            if (obj == null)
                return;

            List<Tour>? tours = obj["tours"]?.ToObject<List<Tour>>();

            if (tours == null)
                return;

            for (int i = 0; i < tours.Count; i++)
            {
                var tour = tours[i];
                if (CreateTourApi(ref tour))
                    AllTours.Add(tour);
            }

            OnPropertyChanged(nameof(AllTours));
            OnPropertyChanged(nameof(DisplayedTours));
        }

        private void AllTours_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DisplayedTours.Clear();
            foreach (var tour in AllTours)
            {
                DisplayedTours.Add(tour);
            }
        }

        public void Export(Uri filePath)
        {
            // Compute file hash => File cannot be edited by user
            byte[] fileData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(AllTours.ToArray()));
            byte[] hashedData = Encrypt(fileData, "TourPlannerSecret");

            // Write all tours as json in given file
            File.WriteAllBytes(filePath.AbsolutePath, hashedData);
        }
        public void Import(Uri filePath)
        {
            bool error = false;

            // Load and decrypt file
            byte[] hashedData = File.ReadAllBytes(filePath.AbsolutePath);
            byte[] fileData = Decrypt(hashedData, "TourPlannerSecret");

            // Parse json format into list of tours
            var toursString = Encoding.UTF8.GetString(fileData);
            var tours = JsonConvert.DeserializeObject<List<Tour>>(toursString);

            if (tours == null)
                throw new Exception("File format is invalid or corrupted!");

            // Check which tours are different => Update
            var duplicates = tours.Where(t => AllTours.Any(t2 => t.Id == t2.Id)).ToList();

            // Ask if override
            if (duplicates.Any())
            {
                bool overrideTours = MessageBox.Show(
                    $"Do you want to override {duplicates.Count} existing tours with the imported tours?",
                    $"{duplicates.Count} duplicates found!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No) == MessageBoxResult.Yes;

                if (overrideTours)
                {
                    for (int i = 0; i < duplicates.Count; i++)
                    {
                        var newTour = duplicates[i];
                        var t = AllTours.Where(t => t.Id == newTour.Id).FirstOrDefault();
                        if (t != null && (!Online || UpdateTourApi(ref newTour)))
                        {
                            AllTours.Remove(t);
                            AllTours.Add(newTour);
                        }
                        else
                        {
                            error = true;
                        }
                    }

                    tours.RemoveAll(t => duplicates.Contains(t));
                }
            }

            // Check which tours are not in current collection => Create
            var newTours = tours.Where(t => !AllTours.Any(t2 => t.Id == t2.Id)).ToList();

            // Ask if they should be created
            if (newTours.Any())
            {
                bool createTours = MessageBox.Show(
                    $"Do you want to create {newTours.Count} new tours from the file?",
                    $"{newTours.Count} new tours found found!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No) == MessageBoxResult.Yes;

                if (createTours)
                {
                    for (int i = 0; i < newTours.Count; i++)
                    {
                        var newTour = newTours[i];
                        newTour.Id = -1;
                        if (!Online || CreateTourApi(ref newTour))
                            AllTours.Add(newTour);
                        else
                            error = true;
                    }

                    tours.RemoveAll(t => newTours.Contains(t));
                }
            }

            OnPropertyChanged(nameof(AllTours));
            OnPropertyChanged(nameof(DisplayedTours));

            if (error)
                MessageBox.Show("Import completed with erros!", "Oh no!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show("Import successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool LoadToursApi()
        {
            try
            {
                // Get tour collection string from api
                var response = _apiService.GetStringAsync("Tour").Result;

                if (response.Item2 != HttpStatusCode.OK)
                    return false;

                // Parse string as collection
                var tours = JsonConvert.DeserializeObject<List<Tour>>(response.Item1);
                if (tours == null)
                    return false;

                AllTours = new ObservableCollection<Tour>(tours);
                AllTours.CollectionChanged += AllTours_CollectionChanged;

                DisplayedTours = new ObservableCollection<Tour>(tours);
                Online = true;

                return true;
            }
            catch (Exception)
            {
                Online = false;
            }
            return false;
        }

        public bool CreateTourApi(ref Tour tour)
        {
            // Send post request
            (string, HttpStatusCode) response = _apiService.PostAsync("Tour", tour).Result;

            // Check if saving was successful
            if (response.Item2 != HttpStatusCode.Created)
                return false;

            // Parse string as tour
            var returnedTour = JsonConvert.DeserializeObject<Tour>(response.Item1);
            if (returnedTour == null)
                return false;

            tour = returnedTour;

            return true;
        }

        public bool UpdateTourApi(ref Tour tour)
        {
            // Send put request
            (string, HttpStatusCode) response = _apiService.PutAsync($"Tour/{tour.Id}", tour).Result;

            // Check if saving was successful
            if (response.Item2 != HttpStatusCode.OK)
                return false;

            // Parse string as tour
            var returnedTour = JsonConvert.DeserializeObject<Tour>(response.Item1);
            if (returnedTour == null)
                return false;

            tour = returnedTour;

            return true;
        }

        public bool DeleteTourApi(int tourId)
        {
            var response = _apiService.DeleteAsync($"Tour/{tourId}").Result;

            return response == HttpStatusCode.OK;
        }

        #region Helper

        private static byte[] Encrypt(byte[] plain, string skey)
        {
            using Aes aes = Aes.Create();

            PadToMultipleOf(ref plain, 16);

            aes.Padding = PaddingMode.None;
            aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(skey));
            aes.IV = new byte[] { 0xcf, 0x5e, 0x46, 0x20, 0x45, 0x5c, 0xd7, 0x19, 0x0f, 0xcb, 0x53, 0xed, 0xe8, 0x74, 0xf1, 0xa8 };

            using MemoryStream msCrypt = new();
            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using CryptoStream cs = new(msCrypt, encryptor, CryptoStreamMode.Write);
            using MemoryStream msPlain = new(plain);

            int data;
            while ((data = msPlain.ReadByte()) != -1)
            {
                cs.WriteByte((byte)data);
            }

            return msCrypt.ToArray();
        }
        private static byte[] Decrypt(byte[] encrypted, string skey)
        {
            using Aes aes = Aes.Create();

            PadToMultipleOf(ref encrypted, 16);

            aes.Padding = PaddingMode.None;
            aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(skey));
            aes.IV = new byte[] { 0xcf, 0x5e, 0x46, 0x20, 0x45, 0x5c, 0xd7, 0x19, 0x0f, 0xcb, 0x53, 0xed, 0xe8, 0x74, 0xf1, 0xa8 };

            using MemoryStream msCrypt = new(encrypted);
            using MemoryStream msPlain = new();
            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using CryptoStream cs = new(msCrypt, decryptor, CryptoStreamMode.Read);

            int data;
            while ((data = cs.ReadByte()) != -1)
            {
                msPlain.WriteByte((byte)data);
            }

            // Remove 0 padding at end of bytes
            var returnArray = msPlain.ToArray().ToList();
            int pad = 0;
            for (int i = returnArray.Count - 1; i > 0; i--)
            {
                if (returnArray[i] == Convert.ToByte(0))
                    pad++;
                else
                    break;
            }
            for (int i = 0; i < pad; i++)
            {
                returnArray.RemoveAt(returnArray.Count - 1);
            }

            return returnArray.ToArray();
        }
        private static void PadToMultipleOf(ref byte[] src, int pad)
        {
            int len = (src.Length + pad - 1) / pad * pad;
            Array.Resize(ref src, len);
        }

        #endregion
    }
}
