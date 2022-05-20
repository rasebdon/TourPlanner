using Moq;
using NUnit.Framework;
using TourPlanner.Client.UI.Services;
using TourPlanner.Client.UI.Services.Reporting;
using TourPlanner.Client.UI.ViewModels;

namespace TourPlanner.Client.UI.Test.ViewModelTests
{
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel;

        // Mocked services
        private Mock<ITourCollectionService> _tourCollectionService;
        private Mock<ITourReportGenerationService> _tourReportGenerationService;
        private Mock<ISummarizeReportGenerationService> _summarizeReportGenerationService;

        [SetUp]
        public void Setup()
        {
            _tourCollectionService = new();
            _tourReportGenerationService = new();
            _summarizeReportGenerationService = new();

            _mainViewModel = new MainViewModel(
                _tourCollectionService.Object,
                _tourReportGenerationService.Object,
                _summarizeReportGenerationService.Object);
        }

        [Test]
        public void SearchCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.SearchCommand);    
        }

        [Test]
        public void ImportCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.ImportCommand);
        }

        [Test]
        public void DeleteTourCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.DeleteTourCommand);
        }

        [Test]
        public void ExitCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.ExitCommand);
        }

        [Test]
        public void ExportCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.ExportCommand);
        }

        [Test]
        public void GenerateSummarizeReportCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.GenerateSummarizeReportCommand);
        }

        [Test]
        public void GenerateTourReportCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.GenerateTourReportCommand);
        }

        [Test]
        public void SettingsCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.SettingsCommand);
        }

        [Test]
        public void AboutCommandNotNull()
        {
            Assert.NotNull(_mainViewModel.AboutCommand);
        }
    }
}