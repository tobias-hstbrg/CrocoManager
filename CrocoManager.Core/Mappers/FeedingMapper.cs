using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace CrocoManager.Core.Mappers
{
    public static class FeedingMapper
    {
        public static Feeding ToModel(
            this FeedingDto dto,
            IEnumerable<FeedingAnimalStatus>? animalFeedingStatus = null,
            FeedingPlan? plan = null)
        {
            return new Feeding
            {
                Id = dto.Id,
                FeedingDate = dto.FeedingDate,
                PerformedByEmail = dto.PerformedByEmail,
                Animals = animalFeedingStatus?.ToList() ?? new List<FeedingAnimalStatus>(),
                FeedingPlan = plan ?? new FeedingPlan()
            };
        }

        public static FeedingDto ToDto(this Feeding model)
        {
            return new FeedingDto
            {
                Id = model.Id,
                FeedingDate = model.FeedingDate,
                FeedingPlanId = model.FeedingPlan.Id,
                PerformedByEmail = model.PerformedByEmail
            };
        }
    }
}
