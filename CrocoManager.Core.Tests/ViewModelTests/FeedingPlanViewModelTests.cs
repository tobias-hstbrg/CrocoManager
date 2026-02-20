using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.DTOs;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class FeedingPlanViewModelTests
    {
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly Mock<IFeedingPlanService> _mockPlanService;
        private readonly FeedingPlanViewModel _viewModel;

        public FeedingPlanViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockNotification = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();
            _mockPlanService = new Mock<IFeedingPlanService>();

            _viewModel = new FeedingPlanViewModel(
                _mockNavigation.Object,
                _mockNotification.Object,
                _mockAuth.Object,
                _mockPlanService.Object);
        }

        [Fact]
        public async Task DeletePlan_ActivePlan_ShouldShowWarningAndNotDelete()
        {
            // Arrange
            var activePlan = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "Active Plan", IsActive = true };
            
            // Act
            await _viewModel.DeletePlanCommand.ExecuteAsync(activePlan);

            // Assert
            _mockNotification.Verify(n => n.ShowWarningAsync("Warnung", "Aktive Futterpläne können nicht gelöscht werden. Bitte zuerst einen anderen Plan aktivieren."), Times.Once);
            _mockPlanService.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task ToggleActivePlan_ShouldDeactivateOthersAndActivateSelected()
        {
            // Arrange
            var oldPlan = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "Old Plan", IsActive = true };
            var newPlan = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "New Plan", IsActive = false };
            
            _viewModel.FeedingPlans.Add(oldPlan);
            _viewModel.FeedingPlans.Add(newPlan);

            // Act
            await _viewModel.ToggleActivePlanCommand.ExecuteAsync(newPlan);

            // Assert
            oldPlan.IsActive.Should().BeFalse();
            newPlan.IsActive.Should().BeTrue();
            _mockPlanService.Verify(s => s.UpdateAsync(oldPlan), Times.Once);
            _mockPlanService.Verify(s => s.UpdateAsync(newPlan), Times.Once);
            _viewModel.ActivePlan.Should().Be(newPlan);
        }

        [Fact]
        public async Task SavePlan_InvalidWeekday_ShouldShowError()
        {
            // Arrange
            _viewModel.Name = "Invalid Plan";
            _viewModel.FoodType = "Meat";
            _viewModel.AmountKg = 10;
            _viewModel.FrequencyPerWeek = 2;
            _viewModel.Weekdays = "Monday, Funday"; // "Funday" is invalid

            // Act
            await _viewModel.SavePlanCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Ungültiger Wochentag", It.IsAny<string>()), Times.Once);
            _mockPlanService.Verify(s => s.AddAsync(It.IsAny<FeedingPlanDto>()), Times.Never);
        }

        [Fact]
        public async Task SavePlan_ValidData_ShouldCallServiceAndClearForm()
        {
            // Arrange
            _viewModel.Name = "Healthy Plan";
            _viewModel.FoodType = "Fish";
            _viewModel.AmountKg = 5;
            _viewModel.FrequencyPerWeek = 3;
            _viewModel.Weekdays = "Montag, Mittwoch, Freitag";

            _mockPlanService.Setup(s => s.AddAsync(It.IsAny<FeedingPlanDto>()))
                .ReturnsAsync(new FeedingPlanDto { Name = "Healthy Plan" });

            // Act
            await _viewModel.SavePlanCommand.ExecuteAsync(null);

            // Assert
            _mockPlanService.Verify(s => s.AddAsync(It.Is<FeedingPlanDto>(p => p.Name == "Healthy Plan")), Times.Once);
            _mockNotification.Verify(n => n.ShowSuccessAsync("Plan hinzugefügt", It.IsAny<string>()), Times.Once);
            _viewModel.Name.Should().BeEmpty();
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
    }
}
