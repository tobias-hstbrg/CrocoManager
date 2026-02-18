using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using CrocoManager.Core.Mappers;
using Xunit;
using FluentAssertions;

namespace CrocoManager.Core.Tests
{
    public class AnimalMappingTests
    {
        [Fact]
        public void AnimalDto_ToModel_Should_MapAllFields()
        {
            // Arrange
            var dto = new AnimalDto
            {
                Id = Guid.NewGuid(),
                Name = "Leo",
                Species = "Lion",
                Age = 5,
                Enclosure = "Savannah",
                Gender = "Male",
                Description = "Healthy adult lion"
            };

            // Act
            var model = dto.ToModel();

            // Assert
            model.Id.Should().Be(dto.Id);
            model.Name.Should().Be("Leo");
            model.Species.Should().Be("Lion");
            model.AgeYears.Should().Be(5);
            model.Enclosure.Should().Be("Savannah");
            model.Gender.Should().Be("Male");
            model.Description.Should().Be("Healthy adult lion");
        }
    }
}
