using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class FeedingViewModelTests
    {
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly Mock<IConnectivityService> _mockConnectivity;
        private readonly Mock<IFeedingService> _mockFeedingService;
        private readonly FeedingViewModel _viewModel;

        public FeedingViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockNotification = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();
            _mockConnectivity = new Mock<IConnectivityService>();
            _mockFeedingService = new Mock<IFeedingService>();

            _mockConnectivity.Setup(c => c.IsConnected).Returns(true);

            _viewModel = new FeedingViewModel(
                _mockNavigation.Object,
                _mockNotification.Object,
                _mockAuth.Object,
                _mockConnectivity.Object,
                _mockFeedingService.Object);
        }

        [Fact]
        public async Task LoadAsync_ShouldPopulateAnimalList()
        {
            // Arrange
            var feeding = new Feeding
            {
                Animals = new List<FeedingAnimalStatus>
                {
                    new FeedingAnimalStatus { Animal = new Animal { Name = "Croc 1" }, WasFed = false }
                }
            };

            _mockFeedingService.Setup(s => s.GetTodayFeedingDraftAsync())
                .ReturnsAsync(feeding);
            _mockFeedingService.Setup(s => s.GetHistoryAsync())
                .ReturnsAsync(new List<FeedingHistoryEntry>());

            // Act
            await _viewModel.LoadCommand.ExecuteAsync(null);

            // Assert
            _viewModel.Animals.Should().HaveCount(1);
            _viewModel.Animals.First().Animal.Name.Should().Be("Croc 1");
        }

        [Fact]
        public async Task SaveAsync_NoAnimalsSelected_ShouldShowError()
        {
            // Arrange
            _viewModel.CurrentFeeding = new Feeding { Animals = new List<FeedingAnimalStatus>() };
            _viewModel.Animals.Clear(); // No animals selected

            // Act
            await _viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Fehler", "Bitte wählen Sie mindestens ein Tier aus."), Times.Once);
            _mockFeedingService.Verify(s => s.SaveFeedingAsync(It.IsAny<Feeding>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SaveAsync_ValidSelection_ShouldCallServiceAndShowSuccess()
        {
            // Arrange
            var animal = new Animal { Id = Guid.NewGuid(), Name = "Croc" };
            var status = new FeedingAnimalStatus { Animal = animal, WasFed = true };
            var feeding = new Feeding { Id = Guid.NewGuid(), Animals = new List<FeedingAnimalStatus> { status } };
            
            _viewModel.CurrentFeeding = feeding;
            _viewModel.Animals.Add(new FeedingAnimalStatusViewModel(status, () => { }));
            
            _mockAuth.Setup(a => a.GetUserEmail()).ReturnsAsync("ranger@test.com");

            // Act
            await _viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            _mockFeedingService.Verify(s => s.SaveFeedingAsync(It.IsAny<Feeding>(), "ranger@test.com"), Times.Once);
            _mockNotification.Verify(n => n.ShowSuccessAsync("Erfolgreich", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void HasSelection_ShouldUpdate_WhenAnimalStatusChanges()
        {
            // Arrange
            bool propertyChangedCalled = false;
            _viewModel.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(_viewModel.HasSelection)) propertyChangedCalled = true; };
            
            var status = new FeedingAnimalStatus { WasFed = false };
            var vm = new FeedingAnimalStatusViewModel(status, () => { 
                // This simulates the toolkit behavior without calling protected methods
                typeof(FeedingViewModel).GetMethod("OnPropertyChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(string) }, null)
                    ?.Invoke(_viewModel, new[] { nameof(_viewModel.HasSelection) });
            });
            _viewModel.Animals.Add(vm);

            // Act
            vm.WasFed = true;

            // Assert
            propertyChangedCalled.Should().BeTrue();
            _viewModel.HasSelection.Should().BeTrue();
        }
    }
}
