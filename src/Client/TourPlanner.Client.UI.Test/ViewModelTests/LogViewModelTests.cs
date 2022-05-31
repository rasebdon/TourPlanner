using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.ViewModels;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Test.ViewModelTests
{
    public class LogViewModelTests
    {
        private LogViewModel _logViewModel;
        private TourEntry _tourentry;

        // Mocked Services
        private Mock<IApiService> _apiService;
        private Mock<ITourCollectionService> _tourCollectionService;
        private Mock<ITourSelectionService> _tourSelectionService;

        [SetUp]
        public void SetUp()
        {
            _apiService = new();
            _tourCollectionService = new();
            _tourSelectionService = new();

            // Tour
            Tour tour = new();
            tour.Id = 0;
            tour.Name = "Test Tour";
            tour.Description = "Test description";
            tour.Distance = 10;
            tour.EstimatedTime = 1;
            tour.TransportType = Common.Models.TransportType.AUTO;

            // Tour Entry
            _tourentry = new TourEntry()
            {
                Id = 1,
                TourId = tour.Id,
                Comment = "comment",
                Difficulty = 1,
                Rating = 1,
                Date = new DateTime(2022, 1, 1),
                Duration = 1,
                Distance = 1
            };
            
            // LogViewModel
            _logViewModel = new LogViewModel(
                _apiService.Object,
                _tourCollectionService.Object,
                _tourSelectionService.Object);
            _logViewModel.SelectedTour = tour;
            _logViewModel.SelectedTourEntry = _tourentry;
        }

        [Test]
        public void AddLogEntryCommand_AddsTourLog()
        {
            // Arrange
            _tourCollectionService.Setup(mock => mock.Online).Returns(true);

            (string, HttpStatusCode) task_result = (JsonConvert.SerializeObject(_tourentry), HttpStatusCode.Created);
            var responseTask = Task.FromResult(task_result);

            TourEntry entry = new TourEntry() { Date = DateTime.Now, Id = -1, TourId = _logViewModel.SelectedTour.Id };
            _apiService.Setup(mock => mock.PostAsync($"Tour/{_logViewModel.SelectedTour.Id}/Entry", entry)).Returns(responseTask);

            // Act
            _logViewModel.AddLogEntryCommand.Execute(null);

            // Assert
            Assert.AreEqual(_logViewModel.Data[0], _tourentry);
            Assert.AreEqual(_logViewModel.SelectedTour.Entries[0], _tourentry);
        }

        [Test]
        public void RemoveLogEntryCommand_DeletesSelectedItem()
        {
            // Arrange
            _logViewModel.Data.Add(_tourentry);
            _apiService.Setup(mock => mock.DeleteAsync($"Tour/Entry/{_logViewModel.SelectedTourEntry.Id}")).Returns(Task.FromResult(HttpStatusCode.OK));

            // Act
            _logViewModel.RemoveLogEntryCommand.Execute(null);

            // Assert
            Assert.IsEmpty(_logViewModel.Data);
            Assert.IsEmpty(_logViewModel.SelectedTour.Entries);
        }

        [Test]
        public void SaveTableCommand_CallsApiService()
        {
            // Arrange
            _tourCollectionService.Setup(mock => mock.Online).Returns(true);

            (string, HttpStatusCode) task_result = (JsonConvert.SerializeObject(_tourentry), HttpStatusCode.OK);
            var responseTask = Task.FromResult(task_result);
            _apiService.Setup(mock => mock.PutAsync($"Tour/Entry/{_logViewModel.SelectedTourEntry.Id}", _logViewModel.SelectedTourEntry)).Returns(responseTask);

            // Act 
            _logViewModel.SaveTableCommand.Execute(null);

            // Assert
            _apiService.Verify(mock => mock.PutAsync($"Tour/Entry/{_logViewModel.SelectedTourEntry.Id}", _logViewModel.SelectedTourEntry), Times.Once);
        }

        [Test]
        public void AddLogEntryCommandNotNull()
        {
            Assert.NotNull(_logViewModel.AddLogEntryCommand);
        }

        [Test]
        public void RemoveLogEntryCommandNotNull()
        {
            Assert.NotNull(_logViewModel.RemoveLogEntryCommand);
        }

        [Test]
        public void SaveTableCommandNotNull()
        {
            Assert.NotNull(_logViewModel.SaveTableCommand);
        }


    }
}
