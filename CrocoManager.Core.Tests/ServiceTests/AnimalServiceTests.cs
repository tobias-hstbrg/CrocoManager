using CrocoManager.Core.DTOs;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Tests.ServiceTests
{
    public class AnimalServiceTests
    {
        private readonly Mock<ISupabaseClientService> _mockSupabase;
        private readonly Mock<AnimalService> _mockService;

        public AnimalServiceTests()
        {
            _mockSupabase = new Mock<ISupabaseClientService>();
            _mockService = new Mock<AnimalService>(_mockSupabase.Object) { CallBase = true };
        }

        [Fact]
        public async Task GetTotalCount_Should_ReturnCorrectCount()
        {
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<AnimalDto> { new(), new(), new() });

            var count = await _mockService.Object.GetTotalCount();

            count.Should().Be(3);
        }

        [Fact]
        public async Task GetBySpeciesAsync_Should_CallFilterWithCorrectParameters()
        {
            var species = "American Crocodile";
            _mockService.Setup(s => s.FilterByAsync("species", species, It.IsAny<Supabase.Postgrest.Constants.Operator>())).ReturnsAsync(new List<AnimalDto>());

            await _mockService.Object.GetBySpeciesAsync(species);

            _mockService.Verify(s => s.FilterByAsync("species", species, It.IsAny<Supabase.Postgrest.Constants.Operator>()), Times.Once);
        }
    }
}
