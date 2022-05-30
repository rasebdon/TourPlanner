using Moq;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Client.UI.ViewModels;
using TourPlanner.Common.Models;

namespace TourPlanner.Client.UI.Test.ViewModelTests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel;

        // Mocked services
        private Mock<ITourCollectionService> _tourCollectionService;
        private Mock<ITourReportGenerationService> _tourReportGenerationService;
        private Mock<ISummarizeReportGenerationService> _summarizeReportGenerationService;
        private Mock<IOpenFileDialogProvider> _openFileDialogProvider;
        private Mock<ISaveFileDialogProvider> _saveFileDialogProvider;

        [SetUp]
        public void Setup()
        {
            _tourCollectionService = new ();
            _tourReportGenerationService = new();
            _summarizeReportGenerationService = new();
            _saveFileDialogProvider = new();
            _openFileDialogProvider = new();


            _mainViewModel = new MainViewModel(
                _tourCollectionService.Object,
                _tourReportGenerationService.Object,
                _summarizeReportGenerationService.Object,
                _saveFileDialogProvider.Object,
                _openFileDialogProvider.Object);
        }

        [Test]
        public void ExportCommand_ShouldTriggerExportMethod()
        {
            // Arrange
            var allTours = new ObservableCollection<Tour>()
                {
                    new Tour() { Name = "Test Tour" },
                    new Tour() { Name = "Test Tour" }
                };
            _tourCollectionService.Setup(mock => mock.AllTours).Returns(allTours);
            _saveFileDialogProvider.Setup(mock => mock.GetFileName()).Returns("test.tours");

            // Act
            _mainViewModel.ExportCommand.Execute(null);

            // Assert
            _tourCollectionService.Verify(mock => mock.Export("test.tours"), Times.Once);
        }

        [Test]
        public void ImportCommand_ShouldTriggerImportMethod()
        {
            // Arrange
            var allTours = new ObservableCollection<Tour>();
            _tourCollectionService.Setup(mock => mock.AllTours).Returns(allTours);
            _openFileDialogProvider.Setup(mock => mock.GetFileName()).Returns("test.tours");

            // Act
            _mainViewModel.ImportCommand.Execute(null);

            // Assert
            _tourCollectionService.Verify(mock => mock.Import("test.tours"), Times.Once);
        }


        [Test]
        public void AboutCommand_ShouldOpenWindow()
        {
            // Act && Assert
            Assert.Throws<System.Windows.Markup.XamlParseException>(() =>
                _mainViewModel.AboutCommand.Execute(null));
        }

        [Test]
        public void GenerateTourReportCommand_ShouldTriggerPdfGeneration()
        {
            // Arrange
            Tour tour = new();
            _mainViewModel.Tour = tour;
            _saveFileDialogProvider.Setup(mock => mock.GetFileName()).Returns("test.pdf");
            
            // Act
            _mainViewModel.GenerateTourReportCommand.Execute(null);

            // Assert
            _tourReportGenerationService.Verify(mock => mock.GenerateReport(tour), Times.Once);
        }

        [Test]
        public void GenerateSummarizeReportCommand_ShouldTriggerPdfGeneration()
        {
            // Arrange
            var allTours = new ObservableCollection<Tour>()
                {
                    new Tour() { Name = "Test Tour" },
                    new Tour() { Name = "Test Tour" }
                };
            _tourCollectionService.Setup(mock => mock.AllTours).Returns(allTours);
            _saveFileDialogProvider.Setup(mock => mock.GetFileName()).Returns("test.pdf");

            // Act
            _mainViewModel.GenerateSummarizeReportCommand.Execute(null);
            
            // Assert
            _summarizeReportGenerationService.Verify(mock => mock.GenerateReport(allTours), Times.Once);
        }

        [Test]
        public void SearchCommand_ShouldFilterEntries()
        {
            // Arrange
            _mainViewModel.SearchText = "Test";
            _tourCollectionService.Setup(mock => mock.AllTours).Returns(
                new ObservableCollection<Tour>()
                {
                    new Tour() { Name = "Test Tour" },
                    new Tour() { Name = "New Tour" }
                });
            _tourCollectionService.Setup(mock => mock.DisplayedTours).Returns(new ObservableCollection<Tour>());
            
            // Act
            _mainViewModel.SearchCommand.Execute(null);

            // Assert
            Assert.AreEqual(1, _tourCollectionService.Object.DisplayedTours.Count);
        }

        [Test]
        public void SearchCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.SearchCommand);    
        }

        [Test]
        public void ImportCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.ImportCommand);
        }

        [Test]
        public void DeleteTourCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.DeleteTourCommand);
        }

        [Test]
        public void ExitCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.ExitCommand);
        }

        [Test]
        public void ExportCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.ExportCommand);
        }

        [Test]
        public void GenerateSummarizeReportCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.GenerateSummarizeReportCommand);
        }

        [Test]
        public void GenerateTourReportCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.GenerateTourReportCommand);
        }

        [Test]
        public void SettingsCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.SettingsCommand);
        }

        [Test]
        public void AboutCommand_NotNull()
        {
            Assert.NotNull(_mainViewModel.AboutCommand);
        }
    }
}