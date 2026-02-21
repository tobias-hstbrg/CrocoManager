using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.DTOs;
using CrocoManager.Core.Mappers;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class ObservationViewModelTests
    {
        private readonly Mock<INavigationService> _mockNav;
        private readonly Mock<INotificationService> _mockNote;
        private readonly Mock<IAuthService> _mockAuth;
        private readonly Mock<IConnectivityService> _mockConnectivity;
        private readonly Mock<ISupabaseClientService> _mockSupabase;
        private readonly Mock<IObservationService> _mockObsService;
        private readonly Mock<IAnimalService> _mockAnimalService;
        private readonly Mock<IFeedingService> _mockFeedingService;
        private readonly Mock<IFeedingPlanService> _mockPlanService;
        private readonly ObservationViewModel _viewModel;

        public ObservationViewModelTests()
        {
            _mockNav = new Mock<INavigationService>();
            _mockNote = new Mock<INotificationService>();
            _mockAuth = new Mock<IAuthService>();
            _mockConnectivity = new Mock<IConnectivityService>();
            _mockSupabase = new Mock<ISupabaseClientService>();
            _mockObsService = new Mock<IObservationService>();
            _mockAnimalService = new Mock<IAnimalService>();
            _mockFeedingService = new Mock<IFeedingService>();
            _mockPlanService = new Mock<IFeedingPlanService>();

            _mockConnectivity.Setup(c => c.IsConnected).Returns(true);

            // Setup ObservationService to return some dummy data to avoid crash during init
            _mockObsService.Setup(s => s.FetchEnvironmentalDataAsync())
                .ReturnsAsync(new EnvironmentalData(DateOnly.FromDateTime(DateTime.Now), TimeSpan.Zero, 25, 60, 22, 7, 30));
            
            _mockAnimalService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<AnimalDto>());
            _mockFeedingService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<FeedingDto>());
            _mockPlanService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<FeedingPlanDto>());

            _viewModel = new ObservationViewModel(
                _mockNav.Object,
                _mockNote.Object,
                _mockAuth.Object,
                _mockConnectivity.Object,
                _mockSupabase.Object,
                _mockObsService.Object,
                _mockAnimalService.Object,
                _mockFeedingService.Object,
                _mockPlanService.Object);
        }

        [Fact]
        public async Task SaveObservation_EmptyNotes_ShouldShowError()
        {
            // Arrange
            var animal = new Animal { Id = Guid.NewGuid(), Name = "Croc" };
            var feeding = new Feeding { Id = Guid.NewGuid(), FeedingDate = DateTime.Now };
            
            // Bypass logic to prevent clearing during setup
            var isFilteringField = typeof(ObservationViewModel).GetField("_isFiltering", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isFilteringField!.SetValue(_viewModel, true);

            _viewModel.SelectedAnimal = animal;
            _viewModel.SelectedFeeding = feeding;
            _viewModel.FeedingBehavior = "Normal gefressen";
            _viewModel.Notes = "";

            isFilteringField!.SetValue(_viewModel, false);

            // Act
            await _viewModel.SaveObservationCommand.ExecuteAsync(null);

            // Assert
            _mockNote.Verify(n => n.ShowErrorAsync("Validierungsfehler", "Notizen sind erforderlich."), Times.AtLeastOnce);
        }

        [Fact]
        public void SelectingFeeding_ShouldFilterAnimals_Bidirectional()
        {
            // Arrange
            var animal1 = new Animal { Id = Guid.NewGuid(), Name = "Fed" };
            var animal2 = new Animal { Id = Guid.NewGuid(), Name = "Hungry" };
            
            var feeding = new Feeding
            {
                Id = Guid.NewGuid(),
                Animals = new List<FeedingAnimalStatus>
                {
                    new FeedingAnimalStatus { Animal = animal1, WasFed = true },
                    new FeedingAnimalStatus { Animal = animal2, WasFed = false }
                }
            };

            var allAnimalsField = typeof(ObservationViewModel).GetField("_allAnimals", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            allAnimalsField!.SetValue(_viewModel, new List<Animal> { animal1, animal2 });

            // Act
            _viewModel.SelectedFeeding = feeding;

            // Assert
            _viewModel.Animals.Should().HaveCount(1);
            _viewModel.Animals.Should().Contain(a => a.Id == animal1.Id);
        }

        [Fact]
        public void SelectingAnimal_ShouldFilterFeedings_Bidirectional()
        {
            // Arrange
            var targetAnimal = new Animal { Id = Guid.NewGuid() };
            var otherAnimal = new Animal { Id = Guid.NewGuid() };

            var feedingWith = new Feeding 
            { 
                Id = Guid.NewGuid(), 
                Animals = new List<FeedingAnimalStatus> { new FeedingAnimalStatus { Animal = targetAnimal, WasFed = true } } 
            };
            var feedingWithout = new Feeding 
            { 
                Id = Guid.NewGuid(), 
                Animals = new List<FeedingAnimalStatus> { new FeedingAnimalStatus { Animal = otherAnimal, WasFed = true } } 
            };

            var allFeedingsField = typeof(ObservationViewModel).GetField("_allFeedings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            allFeedingsField!.SetValue(_viewModel, new List<Feeding> { feedingWith, feedingWithout });

            // Act
            _viewModel.SelectedAnimal = targetAnimal;

            // Assert
            _viewModel.Feedings.Should().HaveCount(1);
            _viewModel.Feedings.Should().Contain(f => f.Id == feedingWith.Id);
        }

        [Fact]
        public async Task SetPermissions_Ranger_ShouldRestrictCreation()
        {
            // Arrange
            _mockAuth.Setup(a => a.GetUserRoleAsync()).ReturnsAsync(UserRole.Ranger);

            // Act
            await _viewModel.InitializeAsync();

            // Assert
            _viewModel.CanCreate.Should().BeFalse();
            _viewModel.IsReadOnly.Should().BeTrue();
        }
    }
}
