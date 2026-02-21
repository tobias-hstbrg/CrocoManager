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
    }
}
