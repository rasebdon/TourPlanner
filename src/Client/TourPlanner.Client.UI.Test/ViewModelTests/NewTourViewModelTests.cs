using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Client.UI.ViewModels;

namespace TourPlanner.Client.UI.Test.ViewModelTests
{
    public class NewTourViewModelTests
    {
        private NewTourViewModel _newTourViewModel;

        // Mocked services
        private Mock<ITourCollectionService> _tourCollectionService;
        private Mock<IApiService> _apiService;
        private Mock<ITourImageService> _tourImageService;
        private Mock<IBitmapImageService> _bitmapImageService;

        [SetUp]
        public void Setup()
        {
            _tourCollectionService = new();
            _apiService = new();
            _tourImageService = new();
            _bitmapImageService = new();

            _tourImageService.Setup(mock => mock.DefaultImage).Returns(Array.Empty<byte>());

            _newTourViewModel = new NewTourViewModel(
                _tourCollectionService.Object,
                _apiService.Object,
                _tourImageService.Object,
                _bitmapImageService.Object);            
        }

        [Test]
        public void TranslateStartAddressCommand_CallsApiService()
        {
            // Arrange
            _newTourViewModel.StartRoad = "Höchstädtplatz";
            _newTourViewModel.StartNumber = "10";
            _newTourViewModel.StartZip = "1020";
            _newTourViewModel.StartCountry = "AT";

            // Act 
            _newTourViewModel.TranslateStartAddress.Execute(null);

            // Assert
            _apiService.Verify(mock => mock.GetStringAsync(
                $"Coordinates/?address={_newTourViewModel.StartRoad},{_newTourViewModel.StartNumber},{_newTourViewModel.StartZip},{_newTourViewModel.StartCountry}"), 
                Times.Once);
        }

        [Test]
        public void TranslateEndAddressCommand_CallsApiService()
        {
            // Arrange
            _newTourViewModel.EndRoad = "Donauinsel";
            _newTourViewModel.EndZip = "1002";
            _newTourViewModel.EndCountry = "AT";

            // Act 
            _newTourViewModel.TranslateEndAddress.Execute(null);

            // Assert
            _apiService.Verify(mock => mock.GetStringAsync(
                $"Coordinates/?address={_newTourViewModel.EndRoad},{_newTourViewModel.EndNumber},{_newTourViewModel.EndZip},{_newTourViewModel.EndCountry}"),
                Times.Once);
        }

        [Test]
        public void TranslateStartAddressCommand_UpdatesStartLatitudeAndLongitude()
        {
            // Arrange
            _newTourViewModel.StartRoad = "Höchstädtplatz";
            _newTourViewModel.StartNumber = "10";
            _newTourViewModel.StartZip = "1020";
            _newTourViewModel.StartCountry = "AT";

            (string, HttpStatusCode) task_result = ("{\"Id\":-1,\"Longitude\":\"50.002\",\"Latitude\":\"50.001\"}", HttpStatusCode.OK);
            var responseTask = Task.FromResult(task_result);

            _apiService.Setup(mock => mock.GetStringAsync(
                $"Coordinates/?address={_newTourViewModel.StartRoad},{_newTourViewModel.StartNumber},{_newTourViewModel.StartZip},{_newTourViewModel.StartCountry}")).Returns(
                responseTask);

            // Act
            _newTourViewModel.TranslateStartAddress.Execute(null);

            // Assert
            Assert.AreEqual(_newTourViewModel.StartLatitude, "50.001");
            Assert.AreEqual(_newTourViewModel.StartLongitude, "50.002");

        }

        [Test]
        public void TranslateEndAddressCommand_UpdatesEndLatitudeAndLongitude()
        {
            // Arrange
            _newTourViewModel.EndRoad = "Donauinsel";
            _newTourViewModel.EndZip = "1002";
            _newTourViewModel.EndCountry = "AT";

            (string, HttpStatusCode) task_result = ("{\"Id\":-1,\"Longitude\":\"50.002\",\"Latitude\":\"50.001\"}", HttpStatusCode.OK);
            var responseTask = Task.FromResult(task_result);

            _apiService.Setup(mock => mock.GetStringAsync(
                $"Coordinates/?address={_newTourViewModel.EndRoad},{_newTourViewModel.EndNumber},{_newTourViewModel.EndZip},{_newTourViewModel.EndCountry}")).Returns(
                responseTask);

            // Act
            _newTourViewModel.TranslateEndAddress.Execute(null);

            // Assert
            Assert.AreEqual(_newTourViewModel.EndLatitude, "50.001");
            Assert.AreEqual(_newTourViewModel.EndLongitude, "50.002");

        }

        [Test]
        public void CreateTourCommandNotNull()
        {
            Assert.NotNull(_newTourViewModel.CreateTour);
        }

        [Test]
        public void TranslateStartAddressCommandNotNull()
        {
            Assert.NotNull(_newTourViewModel.TranslateStartAddress);
        }

        [Test]
        public void TranslateEndAddressCommandNotNull()
        {
            Assert.NotNull(_newTourViewModel.TranslateEndAddress);
        }
    }
}