using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using CrocoManager.Core.Mappers;
using Xunit;
using FluentAssertions;

namespace CrocoManager.Core.Tests.MapperTests
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
                Species = "American Crocodile",
                Age = 5,
                Enclosure = "B",
                Gender = "Male",
                Description = "Very aggresive at times."
            };

            // Act
            var model = dto.ToModel();

            // Assert
            model.Id.Should().Be(dto.Id);
            model.Name.Should().Be("Leo");
            model.Species.Should().Be("American Crocodile");
            model.AgeYears.Should().Be(5);
            model.Enclosure.Should().Be("B");
            model.Gender.Should().Be("Male");
            model.Description.Should().Be("Very aggresive at times.");
        }

        [Fact]
        public void Animal_ToDto_Should_MapAllFields()
        {
            // Arrange
            var model = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Leo",
                Species = "American Crocodile",
                AgeYears = 3,
                Enclosure = "B",
                Gender = "Male",
                Description = "Very aggresive at times."
            };

            var dto = model.ToDto();

            dto.Id.Should().Be(model.Id);
            dto.Name.Should().Be("Leo");
            dto.Species.Should().Be("American Crocodile");
            dto.Age.Should().Be(3);
            dto.Enclosure.Should().Be("B");
            dto.Gender.Should().Be("Male");
            dto.Description.Should().Be("Very aggresive at times.");

        }
    }
}
