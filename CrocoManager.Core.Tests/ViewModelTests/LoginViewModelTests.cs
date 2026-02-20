using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class LoginViewModelTests
    {
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockNotification = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();

            _viewModel = new LoginViewModel(
                _mockNavigation.Object,
                _mockNotification.Object,
                _mockAuth.Object);
        }

        [Fact]
        public async Task LoginUserAsync_EmptyFields_ShouldShowError()
        {
            // Arrange
            _viewModel.Email = "";
            _viewModel.Password = "";

            // Act
            await _viewModel.LoginUserCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Error", It.IsAny<string>()), Times.Once);
            _mockAuth.Verify(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginUserAsync_ValidAdmin_ShouldNavigateToAdmin()
        {
            // Arrange
            _viewModel.Email = "admin@test.com";
            _viewModel.Password = "password";

            var adminSession = new SupabaseSession
            {
                User = new User
                {
                    Email = "admin@test.com",
                    UserMetadata = new UserMetadata { Role = UserRole.Admin }
                }
            };

            _mockAuth.Setup(a => a.LoginAsync(_viewModel.Email, _viewModel.Password))
                .ReturnsAsync(adminSession);

            // Act
            await _viewModel.LoginUserCommand.ExecuteAsync(null);

            // Assert
            _mockNavigation.Verify(n => n.SetRoot("Admin"), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_ValidScientist_ShouldNavigateToAppShell()
        {
            // Arrange
            _viewModel.Email = "scientist@test.com";
            _viewModel.Password = "password";

            var scientistSession = new SupabaseSession
            {
                User = new User
                {
                    Email = "scientist@test.com",
                    UserMetadata = new UserMetadata { Role = UserRole.Scientist }
                }
            };

            _mockAuth.Setup(a => a.LoginAsync(_viewModel.Email, _viewModel.Password))
                .ReturnsAsync(scientistSession);

            // Act
            await _viewModel.LoginUserCommand.ExecuteAsync(null);

            // Assert
            _mockNavigation.Verify(n => n.SetRoot("AppShell"), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidCredentials_ShouldShowError()
        {
            // Arrange
            _viewModel.Email = "wrong@test.com";
            _viewModel.Password = "wrong";

            _mockAuth.Setup(a => a.LoginAsync(_viewModel.Email, _viewModel.Password))
                .ReturnsAsync((SupabaseSession)null!);

            // Act
            await _viewModel.LoginUserCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Anmeldung fehlgeschlagen", It.IsAny<string>()), Times.Once);
            _mockNavigation.Verify(n => n.SetRoot(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginUserAsync_NotAssignedRole_ShouldShowInfo()
        {
            // Arrange
            _viewModel.Email = "new@test.com";
            _viewModel.Password = "password";

            var unassignedSession = new SupabaseSession
            {
                User = new User
                {
                    Email = "new@test.com",
                    UserMetadata = new UserMetadata { Role = UserRole.NotAssigned }
                }
            };

            _mockAuth.Setup(a => a.LoginAsync(_viewModel.Email, _viewModel.Password))
                .ReturnsAsync(unassignedSession);

            // Act
            await _viewModel.LoginUserCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowInfoAsync("Account in Bearbeitung", It.IsAny<string>()), Times.Once);
            _mockNavigation.Verify(n => n.SetRoot(It.IsAny<string>()), Times.Never);
        }
    }
}
