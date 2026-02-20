using CrocoManager.Core.DTOs;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using CrocoManager.Core.Services;
using static Supabase.Postgrest.Constants;

namespace CrocoManager.Services;

public class FeedingService : BaseService<FeedingDto>, IFeedingService
{
    private readonly ISupabaseClientService _supabase;

    public FeedingService(ISupabaseClientService supabaseClient) : base(supabaseClient)
    {
        _supabase = supabaseClient;
    }

    public async Task<Feeding?> GetTodayFeedingDraftAsync()
    {
        var planResponse = await _supabase.Client
            .From<FeedingPlanDto>()
            .Filter("is_active", Operator.Equals, "true")
            .Get();

        var plan = planResponse.Models.SingleOrDefault();

        if (plan == null) return null;

        var animalsResponse = await _supabase.Client
            .From<AnimalDto>()
            .Get();

        var animals = animalsResponse.Models;

        if (animals == null || !animals.Any()) return null;

        return new Feeding
        {
            FeedingDate = DateTime.Now,
            FeedingPlan = Map(plan),
            Animals = animals.Select(a => new FeedingAnimalStatus
            {
                Animal = MapAnimal(a),
                WasFed = false
            }).ToList()
        };
    }

    public async Task SaveFeedingAsync(Feeding feeding, string rangerEmail)
    {
        var feedingDto = new FeedingDto
        {
            Id = Guid.NewGuid(),
            FeedingDate = feeding.FeedingDate,
            FeedingPlanId = feeding.FeedingPlan.Id,
            PerformedByEmail = rangerEmail
        };

        // Insert feeding
        var response = await _supabase.Client
            .From<FeedingDto>()
            .Insert(feedingDto);

        if (response?.Models?.FirstOrDefault() == null)
        {
            throw new Exception("Fütterung konnte nicht gespeichert werden");
        }

        var insertedFeeding = response.Models.First();

        // Batch-Insert aller Tier-Verknüpfungen
        var feedingAnimals = feeding.Animals
            .Select(animal => new FeedingAnimalDto
            {
                FeedingId = insertedFeeding.Id,
                AnimalId = animal.Animal.Id,
                WasFed = animal.WasFed
            })
            .ToList();

        if (feedingAnimals.Any())
        {
            await _supabase.Client
                .From<FeedingAnimalDto>()
                .Insert(feedingAnimals);
        }
    }

    public async Task<List<FeedingHistoryEntry>> GetHistoryAsync()
    {
        var feedings = (await _supabase.Client.From<FeedingDto>().Order( "feeding_date", Ordering.Descending ).Get()).Models;
        if (feedings.Count == 0)
            return [];

        var planIds = feedings.Select(f => f.FeedingPlanId).Distinct().ToList();
        var plans = (await _supabase.Client
            .From<FeedingPlanDto>()
            .Filter("id", Operator.In, planIds)
            .Get()).Models;

        var feedingIds = feedings.Select(f => f.Id).ToList();
        var feedingAnimals = (await _supabase.Client
            .From<FeedingAnimalDto>()
            .Filter("feeding_id", Operator.In, feedingIds)
            .Get()).Models;

        return feedings.Select(f =>
        {
            var plan = plans.Single(p => p.Id == f.FeedingPlanId);
            var animals = feedingAnimals.Where(fa => fa.FeedingId == f.Id).ToList();

            return new FeedingHistoryEntry
            {
                FeedingDate = f.FeedingDate,
                FeedingPlanName = plan.Name,
                FedAnimals = animals.Count(a => a.WasFed),
                TotalAnimals = animals.Count,
                PerformedByEmail = f.PerformedByEmail
            };
        }).ToList();
    }

    public async Task<int> GetCurrentWeekCount()
    {
        try
        {
            var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            var feedingsThisWeek = (await _supabase.Client
                .From<FeedingDto>()
                .Filter("feeding_date", Operator.GreaterThanOrEqual, startOfWeek.ToString("o"))
                .Filter("feeding_date", Operator.LessThan, endOfWeek.ToString("o"))
                .Get()).Models;

            return feedingsThisWeek.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Abrufen der Fütterungen dieser Woche: {ex.Message}");
            return 0;
        }

    }

    private static FeedingPlan Map(FeedingPlanDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        FoodType = dto.FoodType,
        AmountKg = (decimal)dto.AmountKg,
        FrequencyPerWeek = dto.FrequencyPerWeek,
        Weekdays = dto.Weekdays,
        IsActive = dto.IsActive
    };

    private static Animal MapAnimal(AnimalDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Species = dto.Species,
        Enclosure = dto.Enclosure ?? string.Empty,
        AgeYears = dto.Age ?? 0
    };
}
