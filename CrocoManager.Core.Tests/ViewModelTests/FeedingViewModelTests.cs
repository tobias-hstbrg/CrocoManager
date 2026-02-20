using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.Services;
using CrocoManager.Core.ViewModels;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class FeedingViewModelTests
    {
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly Mock<IFeedingService> _mockFeedingService;
        private readonly FeedingViewModel _viewModel;

        public FeedingViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockNotification = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();
            _mockFeedingService = new Mock<IFeedingService>();

            _viewModel = new FeedingViewModel(
                _mockNavigation.Object,
                _mockNotification.Object,
                _mockAuth.Object,
                _mockFeedingService.Object);
        }

        [Fact]
        public async Task LoadAsync_ShouldPopulateAnimalList()
        {
            var fakeFeeding = new Feeding
            {
                Animals = new List<FeedingAnimalStatus>
                {
                    new FeedingAnimalStatus { Animal = new Animal { Name = "Croc 1" }, WasFed = false },
                    new FeedingAnimalStatus { Animal = new Animal { Name = "Croc 2" }, WasFed = false }
                }
            };
            _mockFeedingService.Setup(s => s.GetTodayFeedingDraftAsync()).ReturnsAsync(fakeFeeding);

            //Act
            await _viewModel.LoadCommand.ExecuteAsync(null);

            //Assert
            _viewModel.Animals.Should().HaveCount(2);
            _viewModel.HasCurrentFeeding.Should().BeTrue();
        }

        [Fact]
        public async Task HasSelection_ShouldUpdate_WhenAnimalStatusChanges()
        {
            var fakeFeeding = new Feeding
            {
                Animals = new List<FeedingAnimalStatus>
                {
                    new FeedingAnimalStatus { Animal = new Animal { Name = "Croc 1"}, WasFed = false }
                }
            };
            _mockFeedingService.Setup(s => s.GetTodayFeedingDraftAsync()).ReturnsAsync(fakeFeeding);
            await _viewModel.LoadCommand.ExecuteAsync(null);

            //Act
            _viewModel.Animals[0].WasFed = true;

            //Assert
            _viewModel.HasSelection.Should().BeTrue();
        }

        [Fact]
        public async Task SaveAsync_NoAnimalsSelected_ShouldShowError()
        {
            var fakeFeeding = new Feeding { Animals = new List<FeedingAnimalStatus>() };
            _viewModel.CurrentFeeding = fakeFeeding;

            //Act
            await _viewModel.SaveCommand.ExecuteAsync(null);

            //Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Fehler", "Bitte wählen Sie mindestens ein Tier aus."), Times.Once);
            _mockFeedingService.Verify(s => s.SaveFeedingAsync(It.IsAny<Feeding>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SaveAsync_ValidSelection_ShouldCallServiceAndShowSuccess()
        {
            var fakeFeeding = new Feeding { Animals = new List<FeedingAnimalStatus>() };
            _viewModel.CurrentFeeding = fakeFeeding;
            _mockAuth.Setup(a => a.GetUserEmail()).ReturnsAsync("ranger@everglades.com");

            // Manually adding one animal and selecting it
            _viewModel.Animals.Add(new FeedingAnimalStatusViewModel( new FeedingAnimalStatus {  Animal = new Animal() }, () => { } ) );
            _viewModel.Animals[0].WasFed = true;

            //Act
            await _viewModel.SaveCommand.ExecuteAsync(null);

            //Assert
            _mockFeedingService.Verify(s => s.SaveFeedingAsync(fakeFeeding, "ranger@everglades.com"), Times.Once );
            _mockNotification.Verify(n => n.ShowSuccessAsync("Erfolgreich", "Fütterung gespeichert"), Times.Once );
        }
    }
}
