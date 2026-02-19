using CrocoManager.Core.DTOs;
using CrocoManager.Core.Mappers;
using CrocoManager.Core.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Tests.MapperTests
{
    public class ObservationMappingTest
    {
        [Fact]
        public void Observation_ToEntity_Should_CombineAllDataSources()
        {
            var dto = new ObservationDto
            {
                Id = Guid.NewGuid(),
                FeedingBehavior = "Aggresive",
                Notes = "Animal was very active during feeding.",
                ResearcherEmail = "researcher@example.org",
            };

            var animal = new Animal { Name = "Schnappi", Species = "American Crocodile" };
            var feeding = new Feeding { FeedingDate = DateTime.Now };
            var envData = new EnvironmentalData(
                measurementDate: DateOnly.FromDateTime(DateTime.Now),
                measurementTime: DateTime.UtcNow.TimeOfDay,
                airTemperatureCelsius: 32.5m,
                humidityPercent: 85.0m,
                waterTemperatureCelsius: 28.0m,
                phValue: 7.8m,
                salinityPpt: 30.0m
            );

            var model = dto.ToEntity(animal, feeding, envData);

            model.Id.Should().Be(dto.Id);
            model.Animal.Name.Should().Be("Schnappi");
            model.FeedingBehavior.Should().Be("Aggresive");
            model.Notes.Should().Be(dto.Notes);

            // Check if environmental data has been connected properly
            model.EnvironmentalData.Should().NotBeNull();
            model.EnvironmentalData!.AirTemperatureCelsius.Should().Be(32.5m);
            model.HasEnvironmentalData.Should().BeTrue();
        }

        [Fact]
        public void Observation_ToDto_ShouldExtractCorrectIds()
        {
            var animalId = Guid.NewGuid();
            var feedingId = Guid.NewGuid();
            var envId = Guid.NewGuid();

            var envData = new EnvironmentalData(
                id: envId,
                measurementDate: DateOnly.FromDateTime(DateTime.UtcNow),
                measurementTime: DateTime.UtcNow.TimeOfDay,
                airTemperatureCelsius: 20.0m,
                humidityPercent: 50.0m,
                waterTemperatureCelsius: 18.0m,
                phValue: 7.0m,
                salinityPpt: 10.0m
            );

            var entity = new Observation
            {
                Id = Guid.NewGuid(),
                Animal = new Animal { Id = animalId },
                Feeding = new Feeding { Id = feedingId },
                EnvironmentalData = envData,
                FeedingBehavior = "Normal",
                ResearcherEmail = "test@test.com"
            };

            var dto = entity.ToDto();

            dto.AnimalId.Should().Be(animalId);
            dto.FeedingId.Should().Be(feedingId);
            dto.EnvironmentalDataId.Should().Be(envId);
            dto.ResearcherEmail.Should().Be("test@test.com");

        }
    }
}
