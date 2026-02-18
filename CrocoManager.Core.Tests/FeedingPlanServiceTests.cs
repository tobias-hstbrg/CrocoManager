using CrocoManager.Core.DTOs;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Tests
{
    public class FeedingPlanServiceTests
    {
        private readonly Mock<ISupabaseClientService> _mockSupabaseClient;
        private readonly Mock<FeedingPlanService> _mockService;

        public FeedingPlanServiceTests()
        {
            _mockSupabaseClient = new Mock<ISupabaseClientService>();

            // We create a "Partial Mock" of the service so we can override the DB calls
            // but keep the actual logic of the methods we want to test.
            _mockService = new Mock<FeedingPlanService>(_mockSupabaseClient.Object) { CallBase = true };
        }

        [Fact]
        public async Task GetActivePlanAsync_Should_ReturnPlan_When_ExactlyOneExists()
        {
            var expectedPlan = new FeedingPlanDto { Id = Guid.NewGuid(), Name = "Correct Plan", IsActive = true };
            _mockService.Setup(s => s.FilterByAsync("is_active", "true", Supabase.Postgrest.Constants.Operator.Equals))
                        .ReturnsAsync(new List<FeedingPlanDto> { expectedPlan });

            var result = await _mockService.Object.GetActivePlanAsync();

            result.Should().NotBeNull();
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task GetActivePlanAsync_Should_ThrowException_When_NoPlanIsActive()
        {
            _mockService.Setup(s => s.FilterByAsync("is_active", "true", Supabase.Postgrest.Constants.Operator.Equals))
                        .ReturnsAsync(new List<FeedingPlanDto>());

            var act = () => _mockService.Object.GetActivePlanAsync();

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("No active feeding plan found.");
        }

        [Fact]
        public async Task GetActivePlanAsync_Should_ThrowException_When_MultiplePlansAreActive()
        {
            _mockService.Setup(s => s.FilterByAsync("is_active", "true", Supabase.Postgrest.Constants.Operator.Equals))
                        .ReturnsAsync(new List<FeedingPlanDto> { new(), new() });

            var act = () => _mockService.Object.GetActivePlanAsync();

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Multiple active feeding plans found.");
        }
    }
}
