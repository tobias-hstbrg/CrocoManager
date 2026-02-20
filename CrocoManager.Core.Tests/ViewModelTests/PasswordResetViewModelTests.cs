using CrocoManager.Core.Interfaces;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class PasswordResetViewModelTests
    {
        private readonly Mock<INavigationService> _mockNav;
        private readonly Mock<INotificationService> _mockNote;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly PasswordResetViewModel _viewModel;

        public PasswordResetViewModelTests()
        {
            _mockNav = new Mock<INavigationService>();
            _mockNote = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();

            _viewModel = new PasswordResetViewModel(
                _mockNav.Object,
                _mockNote.Object,
                _mockAuth.Object);
        }

        [Fact]
        public async Task ResetPassword_WeakPassword_ShouldShowValidationError()
        {
            // Arrange
            _viewModel.Email = "test@croco.com";
            _viewModel.Password = "weak";
            _viewModel.PasswordCheck = "weak";

            // Act
            await _viewModel.ResetPasswordCommand.ExecuteAsync(null);

            // Assert
            _mockNote.Verify(n => n.ShowWarningAsync("Passwort-Richtlinien", It.IsAny<string>()), Times.Once);
            _mockAuth.Verify(a => a.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResetPassword_MismatchedPasswords_ShouldShowError()
        {
            // Arrange
            _viewModel.Email = "test@croco.com";
            _viewModel.Password = "Strong123!";
            _viewModel.PasswordCheck = "Different123!";

            // Act
            await _viewModel.ResetPasswordCommand.ExecuteAsync(null);

            // Assert
            _mockNote.Verify(n => n.ShowWarningAsync("Fehler", "Das Passwort stimmt nicht überein"), Times.Once);
            _mockAuth.Verify(a => a.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("short1!")] // Too short
        [InlineData("nonumberextra")] // No number/special/upper
        [InlineData("ONLYUPPER1!")] // No lower
        [InlineData("onlylower1!")] // No upper
        public async Task ResetPassword_InvalidComplexity_ShouldShowWarning(string invalidPassword)
        {
            // Arrange
            _viewModel.Email = "test@croco.com";
            _viewModel.Password = invalidPassword;
            _viewModel.PasswordCheck = invalidPassword;

            // Act
            await _viewModel.ResetPasswordCommand.ExecuteAsync(null);

            // Assert
            _mockNote.Verify(n => n.ShowWarningAsync("Passwort-Richtlinien", It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ValidPassword_ShouldCallServiceAndNavigate()
        {
            // Arrange
            _viewModel.Email = "test@croco.com";
            _viewModel.Password = "Valid123!";
            _viewModel.PasswordCheck = "Valid123!";

            _mockAuth.Setup(a => a.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.ResetPasswordCommand.ExecuteAsync(null);

            // Assert
            _mockAuth.Verify(a => a.ResetPasswordAsync("test@croco.com", "Valid123!"), Times.Once);
            _mockNav.Verify(n => n.SetRoot("Login"), Times.Once);
        }
    }
}
