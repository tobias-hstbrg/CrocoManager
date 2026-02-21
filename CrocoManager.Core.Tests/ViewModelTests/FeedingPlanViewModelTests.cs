using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.DTOs;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class FeedingPlanViewModelTests
    {
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly Mock<IConnectivityService> _mockConnectivity;
        private readonly Mock<IFeedingPlanService> _mockFeedingPlanService;
        private readonly FeedingPlanViewModel _viewModel;

        public FeedingPlanViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockNotification = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();
            _mockConnectivity = new Mock<IConnectivityService>();
            _mockFeedingPlanService = new Mock<IFeedingPlanService>();

            _mockConnectivity.Setup(c => c.IsConnected).Returns(true);

            _viewModel = new FeedingPlanViewModel(
                _mockNavigation.Object,
                _mockNotification.Object,
                _mockAuth.Object,
                _mockConnectivity.Object,
                _mockFeedingPlanService.Object);
        }

        [Fact]
        public async Task SetPermissions_Scientist_ShouldRestrictActions()
        {
            // Arrange
            _mockAuth.Setup(a => a.GetUserRoleAsync()).ReturnsAsync(UserRole.Scientist);

            // Act
            await _viewModel.InitializeAsync();

            // Assert
            _viewModel.CanCreate.Should().BeFalse();
            _viewModel.CanEdit.Should().BeFalse();
            _viewModel.CanDelete.Should().BeFalse();
            _viewModel.IsReadOnly.Should().BeTrue();
        }
        [Fact]
        public async Task SavePlan_EmptyName_ShouldShowError()
        {
            // Arrange
            _viewModel.Name = "";

            // Act
            await _viewModel.SavePlanCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Validierungsfehler", "Bitte geben Sie einen Namen ein."), Times.Once);
        }

        [Fact]
        public async Task SavePlan_InvalidWeekday_ShouldShowError()
        {
            // Arrange
            _viewModel.Name = "Test Plan";
            _viewModel.FoodType = "Meat";
            _viewModel.AmountKg = 5;
            _viewModel.FrequencyPerWeek = 1;
            _viewModel.Weekdays = "InvalidDay";

            // Act
            await _viewModel.SavePlanCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Ungültiger Wochentag", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SavePlan_ValidData_ShouldCallServiceAndClearForm()
        {
            // Arrange
            _viewModel.Name = "Valid Plan";
            _viewModel.FoodType = "Fish";
            _viewModel.AmountKg = 2.5;
            _viewModel.FrequencyPerWeek = 3;
            _viewModel.Weekdays = "Montag, Mittwoch, Freitag";

            _mockFeedingPlanService.Setup(s => s.AddAsync(It.IsAny<FeedingPlanDto>()))
                .ReturnsAsync(new FeedingPlanDto { Name = "Valid Plan" });

            // Act
            await _viewModel.SavePlanCommand.ExecuteAsync(null);

            // Assert
            _mockFeedingPlanService.Verify(s => s.AddAsync(It.Is<FeedingPlanDto>(p => p.Name == "Valid Plan")), Times.Once);
            _viewModel.Name.Should().BeEmpty();
        }

        [Fact]
        public async Task DeletePlan_ActivePlan_ShouldShowWarningAndNotDelete()
        {
            // Arrange
            var activePlan = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "Active", IsActive = true };

            // Act
            await _viewModel.DeletePlanCommand.ExecuteAsync(activePlan);

            // Assert
            _mockNotification.Verify(n => n.ShowWarningAsync("Warnung", It.Is<string>(s => s.Contains("Aktive Futterpläne können nicht gelöscht werden"))), Times.Once);
            _mockFeedingPlanService.Verify(s => s.DeleteAsync(activePlan.Id), Times.Never);
        }

        [Fact]
        public async Task ToggleActivePlan_ShouldDeactivateOthersAndActivateSelected()
        {
            // Arrange
            var plan1 = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "Plan 1", IsActive = true };
            var plan2 = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "Plan 2", IsActive = false };
            _viewModel.FeedingPlans.Add(plan1);
            _viewModel.FeedingPlans.Add(plan2);

            // Act
            await _viewModel.ToggleActivePlanCommand.ExecuteAsync(plan2);

            // Assert
            _mockFeedingPlanService.Verify(s => s.UpdateAsync(It.Is<FeedingPlanDto>(p => p.Id == plan1.Id && !p.IsActive)), Times.Once);
            _mockFeedingPlanService.Verify(s => s.UpdateAsync(It.Is<FeedingPlanDto>(p => p.Id == plan2.Id && p.IsActive)), Times.Once);
        }
    }
}
