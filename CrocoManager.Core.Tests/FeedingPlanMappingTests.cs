using CrocoManager.Core.DTOs;
using CrocoManager.Core.Mappers;
using CrocoManager.Core.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Tests
{
    public class FeedingPlanMappingTests
    {
        [Fact]
        public void FeedingPlanDto_ToModel_Should_MapAllFields()
        {
            var dto = new FeedingPlanDto
            {
                Id = Guid.NewGuid(),
                Name = "Winter Plan",
                FoodType = "Rindfleisch",
                AmountKg = 4.5,
                FrequencyPerWeek = 3,
                IsActive = true,
                Weekdays = new List<Weekday> { Weekday.Montag, Weekday.Mittwoch },
                Description = "Spezialplan"
            };

            var model = dto.ToModel();

            model.Id.Should().Be(dto.Id);
            model.Name.Should().Be(dto.Name);
            model.FoodType.Should().Be(dto.FoodType);
            model.AmountKg.Should().Be(4.5m);
            model.FrequencyPerWeek.Should().Be(dto.FrequencyPerWeek);
            model.IsActive.Should().Be(dto.IsActive);
            model.Weekdays.Should().ContainInOrder(Weekday.Montag, Weekday.Mittwoch);
            model.Description.Should().Be(dto.Description);
        }

        [Fact]
        public void FeedingPlan_ToDto_Should_MapAllFields()
        {
            var model = new FeedingPlan
            {
                Id = Guid.NewGuid(),
                Name = "Sommer-Plan",
                FoodType = "Fish",
                AmountKg = 2.0m,
                FrequencyPerWeek = 7,
                IsActive = false,
                Description = "Leichtekost",
                Weekdays = new List<Weekday> { Weekday.Freitag }
            };

            var dto = model.ToDto();

            dto.Id.Should().Be(model.Id);
            dto.Name.Should().Be(model.Name);
            dto.AmountKg.Should().Be(2.0);
            dto.FoodType.Should().Be(model.FoodType);
            dto.FrequencyPerWeek.Should().Be(model.FrequencyPerWeek);
            dto.IsActive.Should().Be(model.IsActive);
            dto.Description.Should().Be(model.Description);
            dto.Weekdays.Should().BeEquivalentTo(model.Weekdays);

        }
    }
}
