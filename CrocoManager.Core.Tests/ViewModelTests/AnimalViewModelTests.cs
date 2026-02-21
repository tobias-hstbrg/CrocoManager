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
    public class AnimalViewModelTests
    {
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly Mock<IConnectivityService> _mockConnectivity;
        private readonly Mock<IAnimalService> _mockAnimalService;
        private readonly AnimalViewModel _viewModel;

        public AnimalViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockNotification = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();
            _mockConnectivity = new Mock<IConnectivityService>();
            _mockAnimalService = new Mock<IAnimalService>();

            _mockConnectivity.Setup(c => c.IsConnected).Returns(true);

            _viewModel = new AnimalViewModel(
                _mockNavigation.Object,
                _mockNotification.Object,
                _mockAuth.Object,
                _mockConnectivity.Object,
                _mockAnimalService.Object);
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
        public async Task SetPermissions_Ranger_ShouldAllowActions()
        {
            // Arrange
            _mockAuth.Setup(a => a.GetUserRoleAsync()).ReturnsAsync(UserRole.Ranger);

            // Act
            await _viewModel.InitializeAsync();

            // Assert
            _viewModel.CanCreate.Should().BeTrue();
            _viewModel.CanEdit.Should().BeTrue();
            _viewModel.CanDelete.Should().BeTrue();
            _viewModel.IsReadOnly.Should().BeFalse();
        }

        [Fact]
        public async Task SaveAnimal_EmptyName_ShouldShowError()
        {
            // Arrange
            _mockAuth.Setup(a => a.GetUserRoleAsync()).ReturnsAsync(UserRole.Ranger);
            await _viewModel.InitializeAsync();
            _viewModel.Name = "";

            // Act
            await _viewModel.SaveAnimalCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Validierungsfehler", "Bitte geben Sie einen Namen ein."), Times.Once);
            _mockAnimalService.Verify(a => a.AddAsync(It.IsAny<AnimalDto>()), Times.Never);
        }

        [Fact]
        public async Task SaveAnimal_NegativeAge_ShouldShowError()
        {
            // Arrange
            _mockAuth.Setup(a => a.GetUserRoleAsync()).ReturnsAsync(UserRole.Ranger);
            await _viewModel.InitializeAsync();
            _viewModel.Name = "Test Croco";
            _viewModel.Species = "Amerikanischer Alligator";
            _viewModel.Gender = "Männlich";
            _viewModel.Age = -5;

            // Act
            await _viewModel.SaveAnimalCommand.ExecuteAsync(null);

            // Assert
            _mockNotification.Verify(n => n.ShowErrorAsync("Validierungsfehler", "Das Alter muss eine positive Zahl sein."), Times.Once);
            _mockAnimalService.Verify(a => a.AddAsync(It.IsAny<AnimalDto>()), Times.Never);
        }
    }
}
