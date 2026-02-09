using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Mappers
{
    public static class PlanMapper
    {
        public static FeedingPlan ToModel(this FeedingPlan plan)
        {
            return new FeedingPlan
            {
                Id = plan.Id,
                AmountKg = (decimal)plan.AmountKg,
                FoodType = plan.FoodType,
                FrequencyPerWeek = plan.FrequencyPerWeek,
                IsActive = plan.IsActive,
                Name = plan.Name,
                Weekdays = plan.Weekdays,
            };
        }

        public static FeedingPlanDto ToDto(this FeedingPlan plan)
        {
            return new FeedingPlanDto
            {
                Id = plan.Id,
                Description = plan.Description,
                FoodType = plan.FoodType,
                FrequencyPerWeek = plan.FrequencyPerWeek,
                IsActive = plan.IsActive,
                Name = plan.Name,
                AmountKg = (double)plan.AmountKg,
                Weekdays = [.. plan.Weekdays],

            };
        }
    }
}
