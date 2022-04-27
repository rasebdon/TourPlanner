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
        public void Test1()
        {
            Assert.Pass();
        }
    }
}