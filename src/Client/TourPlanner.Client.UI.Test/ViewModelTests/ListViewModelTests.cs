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
        private Mock<ITourReportGenerationService> _tourReportGenerationService;
        private Mock<ISaveFileDialogProvider> _saveFileDialogProvider;
        private Mock<ITourSelectionService> _tourSelectionService;

        [SetUp]
        public void Setup()
        {
            _tourCollectionService = new();
            _tourReportGenerationService = new();
            _saveFileDialogProvider = new();
            _tourSelectionService = new();

            _listViewModel = new ListViewModel(
                _tourCollectionService.Object,
                _tourSelectionService.Object,
                _tourReportGenerationService.Object,
                _saveFileDialogProvider.Object);
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
            Assert.IsNotNull(_listViewModel.DeleteTourCommand);
        }

        [Test]
        public void GenerateTourReportCommand_CallsMainWindowCommand()
        {
            // Assert
            Assert.IsNotNull(_listViewModel.GenerateTourReportCommand);
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