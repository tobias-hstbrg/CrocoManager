using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.ViewModels;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace CrocoManager.Core.Tests.ViewModelTests
{
    public class ObservationViewModelTests
    {
        private readonly Mock<INavigationService> _mockNav;
        private readonly Mock<INotificationService> _mockNote;
        private readonly Mock<IAuthService> _mockAuth;
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
            _mockSupabase = new Mock<ISupabaseClientService>();
            _mockObsService = new Mock<IObservationService>();
            _mockAnimalService = new Mock<IAnimalService>();
            _mockFeedingService = new Mock<IFeedingService>();
            _mockPlanService = new Mock<IFeedingPlanService>();

            // Setup ObservationService to return some dummy data to avoid crash during init
            _mockObsService.Setup(s => s.FetchEnvironmentalDataAsync())
                .ReturnsAsync(new EnvironmentalData(DateOnly.FromDateTime(DateTime.Now), TimeSpan.Zero, 25, 60, 22, 7, 30));

            _viewModel = new ObservationViewModel(
                _mockNav.Object,
                _mockNote.Object,
                _mockAuth.Object,
                _mockSupabase.Object,
                _mockObsService.Object,
                _mockAnimalService.Object,
                _mockFeedingService.Object,
                _mockPlanService.Object);
        }

        [Fact]
        public async Task SaveObservation_EmptyNotes_ShouldShowError()
        {
            // T-OBS-05 Requirement
            // Arrange
            _viewModel.SelectedAnimal = new Animal { Name = "Croc" };
            _viewModel.SelectedFeeding = new Feeding { FeedingDate = DateTime.Now };
            _viewModel.FeedingBehavior = "Normal gefressen";
            _viewModel.Notes = ""; // Trigger the error

            // Act
            await _viewModel.SaveObservationCommand.ExecuteAsync(null);

            // Assert
            _mockNote.Verify(n => n.ShowErrorAsync("Validierungsfehler", "Notizen sind erforderlich."), Times.Once);
            _mockSupabase.Verify(s => s.Client, Times.Never); // Should not proceed to Supabase
        }

        [Fact]
        public void SetPermissions_Ranger_ShouldRestrictCreation()
        {
            // Arrange
            _mockAuth.Setup(a => a.GetUserRoleAsync()).ReturnsAsync(UserRole.Ranger);

            // Act
            _viewModel.InitializeAsync().Wait();

            // Assert
            _viewModel.CanCreate.Should().BeFalse();
            _viewModel.IsReadOnly.Should().BeTrue();
        }
    }
}
