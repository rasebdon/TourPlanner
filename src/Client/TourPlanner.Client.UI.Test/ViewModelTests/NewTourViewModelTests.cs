using Moq;
using NUnit.Framework;
using System;
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
        public void CreateTourCommandNotNull()
        {
            Assert.NotNull(_newTourViewModel.CreateTour);
        }

    }
}