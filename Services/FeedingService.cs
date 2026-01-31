using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class FeedingService
    {
        private readonly SupabaseClientService _supabase;

        public FeedingService(SupabaseClientService supabaseClient)
        {
            _supabase = supabaseClient;
        }

        public async Task<List<Feeding>> GetAllAsync()
        {
            var feedingDtos = (await _supabase.Client.From<FeedingDto>().Get()).Models;

            if (feedingDtos.Count == 0)
                return [];

            var feedingIds = feedingDtos.Select(entry => entry.Id).ToList();
            var planIds = feedingDtos.Select(entry => entry.FeedingPlanId).Distinct().ToList();

            var plans = (await _supabase.Client.From<FeedingPlanDto>()
                .Filter("Id", Supabase.Postgrest.Constants.Operator.In, planIds)
                .Get()).Models;

            var feedingAnimals = (await _supabase.Client.From<FeedingAnimalDto>()
                .Filter("feeding_id", Supabase.Postgrest.Constants.Operator.In, feedingIds)
                .Get()).Models;

            var animalIds = feedingAnimals.Select(entry => entry.AnimalId).Distinct().ToList();

            var animals = (await _supabase.Client.From<AnimalDto>()
                .Filter("Id", Supabase.Postgrest.Constants.Operator.In, animalIds)
                .Get()).Models;

            return Assemble(feedingDtos, plans, feedingAnimals, animals);
        }

        private static List<Feeding> Assemble( List<FeedingDto> feedings, List<FeedingPlanDto> plans, List<FeedingAnimalDto> feedingAnimals, List<AnimalDto> animals)
        {
            return feedings.Select(entry =>
            {
                var plan = plans.Single(p => p.Id == entry.FeedingPlanId);

                var fedAnimalIds = feedingAnimals.Where(fa => fa.FeedingId == entry.Id && fa.WasFed)
                    .Select(fa => fa.AnimalId)
                    .ToHashSet();

                var fedAnimals = animals
                    .Where(a => fedAnimalIds.Contains(a.Id))
                    .ToList();

                return new Feeding
                {
                    Id = entry.Id,
                    FeedingDate = entry.FeedingDate,
                    PerformedByEmail = entry.PerformedByEmail,
                    FeedingPlan = Map(plan),
                    Animals = fedAnimals
                };
            }).ToList();
        }

        private static FeedingPlan Map(FeedingPlanDto dto)
        {
            return new FeedingPlan
            {
                Id = dto.Id,
                Name = dto.Name,
                FoodType = dto.FoodType,
                AmountKg = ((decimal)dto.AmountKg),
                FrequencyPerWeek = dto.FrequencyPerWeek,
                Weekdays = dto.Weekdays,
                IsActive = dto.IsActive
            };
        }
    }
}
