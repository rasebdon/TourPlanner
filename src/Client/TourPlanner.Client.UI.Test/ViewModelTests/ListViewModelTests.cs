using Moq;
using NUnit.Framework;
using System;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Client.UI.ViewModels;

namespace TourPlanner.Client.UI.Test.ViewModelTests
{
    public class ListViewModelTests
    {
        private ListViewModel _listViewModel;

        // Mocked services
        private Mock<ITourCollectionService> _tourCollectionService;
        private Mock<IApiService> _apiService;
        private Mock<ITourImageService> _tourImageService;
        private Mock<ITourReportGenerationService> _tourReportGenerationService;
        private Mock<ISummarizeReportGenerationService> _summarizeReportGenerationService;
        private Mock<IBitmapImageService> _bitmapImageService;
        private Mock<IOpenFileDialogProvider> _openFileDialogProvider;
        private Mock<ISaveFileDialogProvider> _saveFileDialogProvider;

        // Mocked View models
        private Mock<LogViewModel> _logViewModel;
        private Mock<TourViewModel> _tourViewModel;
        private Mock<MainViewModel> _mainViewModel;


        [SetUp]
        public void Setup()
        {
            _tourCollectionService = new();
            _apiService = new();
            _tourImageService = new();
            _tourReportGenerationService = new();
            _summarizeReportGenerationService = new();
            _bitmapImageService = new();
            _saveFileDialogProvider = new();
            _openFileDialogProvider = new();


            _logViewModel = new Mock<LogViewModel>(_apiService.Object, _tourCollectionService.Object);
            _tourViewModel = new Mock<TourViewModel>(_tourCollectionService.Object, _tourImageService.Object, _bitmapImageService.Object);
            _mainViewModel = new Mock<MainViewModel>(
                _tourCollectionService.Object,
                _tourReportGenerationService.Object, 
                _summarizeReportGenerationService.Object,
                _saveFileDialogProvider.Object,
                _openFileDialogProvider.Object);


            _tourImageService.Setup(mock => mock.DefaultImage).Returns(Array.Empty<byte>());

            _listViewModel = new ListViewModel(
                _tourCollectionService.Object,
                _logViewModel.Object,
                _tourViewModel.Object,
                _mainViewModel.Object);
        }

        [Test]
        public void AddListPointCommand_OpensNewTourWindow()
        {
            // Arrange
            _tourCollectionService.Setup(mock => mock.Online).Returns(true);

            // Act & Assert         
            Assert.Throws<InvalidOperationException>(() =>
            _listViewModel.AddListPoint.Execute(null));

        }

        [Test]
        public void DeleteTourCommand_CallsMainWindowCommand()
        {
            // Assert
            Assert.AreEqual(_listViewModel.DeleteTourCommand, _mainViewModel.Object.DeleteTourCommand);
        }

        [Test]
        public void GenerateTourReportCommand_CallsMainWindowCommand()
        {
            // Assert
            Assert.AreEqual(_listViewModel.GenerateTourReportCommand, _mainViewModel.Object.GenerateTourReportCommand);
        }

        [Test]
        public void AddListPointCommandNotNull()
        {
            Assert.NotNull(_listViewModel.AddListPoint);    
        }

        [Test]
        public void GenerateTourReportCommandNotNull()
        {
            Assert.NotNull(_listViewModel.GenerateTourReportCommand);
        }

        [Test]
        public void DeleteTourCommandNotNull()
        {
            Assert.NotNull(_listViewModel.DeleteTourCommand);
        }
    }
}