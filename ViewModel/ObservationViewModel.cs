using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.DTOs;
using CrocoManager.Mappers;
using CrocoManager.Models;
using CrocoManager.Services;
using System.Collections.ObjectModel;
using static Supabase.Postgrest.Constants;

namespace CrocoManager.ViewModel
{
    public partial class ObservationViewModel : BaseViewModel
    {
        private readonly ObservationService _observationService;
        private readonly AnimalService _animalService;
        private readonly FeedingService _feedingService;
        private readonly SupabaseClientService _supabase;

        [ObservableProperty]
        private decimal airTemperature;

        [ObservableProperty]
        private decimal humidity;

        [ObservableProperty]
        private decimal waterTemperature;

        [ObservableProperty]
        private decimal phValue;

        [ObservableProperty]
        private decimal salinity;

        [ObservableProperty]
        private ObservableCollection<Animal> animals;

        [ObservableProperty]
        private Animal? selectedAnimal;

        [ObservableProperty]
        private ObservableCollection<Feeding> feedings;

        [ObservableProperty]
        private ObservableCollection<string> feedingsDisplayNames = new();

        [ObservableProperty]
        private Feeding? selectedFeeding;

        [ObservableProperty]
        private ObservableCollection<string> feedingBehaviors;

        [ObservableProperty]
        private string? feedingBehavior;

        [ObservableProperty]
        private string? notes;

        [ObservableProperty]
        private ObservableCollection<Observation> recentObservations;

        public ObservationViewModel(IServiceProvider serviceProvider,SupabaseClientService supabase, ObservationService observationService, AnimalService animalService, FeedingService feedingService)
            : base(serviceProvider)
        {
            _observationService = observationService;
            _animalService = animalService;
            _feedingService = feedingService;
            _supabase = supabase;

            Animals = new();
            Feedings = new();
            RecentObservations = new();
            FeedingBehaviors = new()
            {
                "Normal gefressen",
                "Langsam gefressen",
                "Aggressiv gefressen",
                "Futter verweigert"
            };

            _ = LoadAsync();
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            try
            {
                var env = await _observationService.FetchEnvironmentalDataAsync();
                AirTemperature = env.AirTemperatureCelsius;
                Humidity = env.HumidityPercent;
                WaterTemperature = env.WaterTemperatureCelsius;
                PhValue = env.PhValue;
                Salinity = env.SalinityPpt;

                var animalDtos = await _animalService.GetAllAsync();
                Animals.Clear();
                foreach (var dto in animalDtos)
                {
                    Animals.Add(dto.ToModel());
                }

                await LoadFeedingsAsync();
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync(
                    "Fehler beim Laden",
                    ex.Message);
            }
        }

        private async Task LoadFeedingsAsync()
        {
            try
            {
                var feedingDtos = await _feedingService.GetAllAsync();
                Feedings.Clear();

                if (!feedingDtos.Any())
                    return;


                var planIds = feedingDtos.Select(f => f.FeedingPlanId).Distinct().ToList();
                var planDtos = (await  _supabase.Client
                    .From<FeedingPlanDto>()
                    .Filter("id", Operator.In, planIds)
                    .Get()).Models;

                // Mapper für Pläne
                var plans = planDtos.ToDictionary(p => p.Id, p => new FeedingPlan
                {
                    Id = p.Id,
                    Name = p.Name,
                    FoodType = p.FoodType,
                    AmountKg = (decimal)p.AmountKg,
                    FrequencyPerWeek = p.FrequencyPerWeek,
                    Weekdays = p.Weekdays,
                    IsActive = p.IsActive
                });

                var feedingIds = feedingDtos.Select(f => f.Id).ToList();
                var feedingAnimalDtos = (await _supabase.Client
                    .From<FeedingAnimalDto>()
                    .Filter("feeding_id", Operator.In, feedingIds)
                    .Get()).Models;

                // Optional: Tiere mappen (falls AnimalDto enthalten ist, sonst extra Query)
                var animalsDict = feedingAnimalDtos
                    .GroupBy(fa => fa.FeedingId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(fa => new FeedingAnimalStatus
                        {
                            Animal = new Animal
                            {
                                Id = fa.AnimalId,
                            },
                            WasFed = fa.WasFed
                        }).ToList()
                    );

                foreach (var dto in feedingDtos.OrderByDescending(f => f.FeedingDate))
                {
                    plans.TryGetValue(dto.FeedingPlanId, out var plan);
                    animalsDict.TryGetValue(dto.Id, out var animals);

                    Feedings.Add(dto.ToModel(animals, plan));
                }
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync(
                    "Fehler beim Laden der Fütterungen",
                    ex.Message);
            }
        }


        [RelayCommand]
        private async Task SaveObservation()
        {
            if (SelectedAnimal == null)
            {
                await NotificationService.ShowErrorAsync("Fehler", "Bitte ein Tier auswählen.");
                return;
            }

            if (SelectedFeeding == null)
            {
                await NotificationService.ShowErrorAsync("Fehler", "Bitte eine Fütterung auswählen.");
                return;
            }

            if (string.IsNullOrWhiteSpace(FeedingBehavior))
            {
                await NotificationService.ShowErrorAsync("Fehler", "Bitte das Fütterungsverhalten auswählen.");
                return;
            }

            IsBusy = true;

            try
            {
                var now = DateTime.Now;

                var environmentalData = new EnvironmentalData(
                    measurementDate: DateOnly.FromDateTime(now),
                    measurementTime: now.TimeOfDay,
                    airTemperatureCelsius: AirTemperature,
                    humidityPercent: Humidity,
                    waterTemperatureCelsius: WaterTemperature,
                    phValue: PhValue,
                    salinityPpt: Salinity
                );

                var environmentalDto = new EnvironmentalDataDto
                {
                    Id = Guid.NewGuid(),
                    MeasurementDate = environmentalData.MeasurementDate,
                    MeasurementTime = environmentalData.MeasurementTime,
                    AirTemperatureCelsius = environmentalData.AirTemperatureCelsius,
                    HumidityPercent = environmentalData.HumidityPercent,
                    WaterTemperatureCelsius = environmentalData.WaterTemperatureCelsius,
                    PhValue = environmentalData.PhValue,
                    SalinityPpt = environmentalData.SalinityPpt
                };

                var envResponse = await _supabase.Client
                    .From<EnvironmentalDataDto>()
                    .Insert(environmentalDto);

                var createdEnvDto = envResponse.Models?.FirstOrDefault();
                if (createdEnvDto == null)
                {
                    await NotificationService.ShowErrorAsync("Fehler", "Die Umweltdaten konnten nicht gespeichert werden.");
                    return;
                }

                var observation = new Observation
                {
                    Id = Guid.NewGuid(),
                    Animal = SelectedAnimal,
                    Feeding = SelectedFeeding,
                    EnvironmentalData = environmentalData,
                    FeedingBehavior = FeedingBehavior,
                    Notes = Notes ?? string.Empty,
                    ResearcherEmail = _supabase.Client.Auth.CurrentUser.Email
                };

                var observationDto = observation.ToDto();
                observationDto.EnvironmentalDataId = createdEnvDto.Id;

                var obsResponse = await _supabase.Client
                    .From<ObservationDto>()
                    .Insert(observationDto);

                var createdObservationDto = obsResponse.Models?.FirstOrDefault();
                if (createdObservationDto != null)
                {
                    RecentObservations.Add(createdObservationDto.ToEntity(
                        animal: SelectedAnimal,
                        feeding: SelectedFeeding,
                        environmentalData: environmentalData
                    ));

                    await NotificationService.ShowSuccessAsync(
                        "Beobachtung gespeichert",
                        "Die Observation wurde erfolgreich gespeichert.");
                }
                else
                {
                    await NotificationService.ShowErrorAsync(
                        "Fehler",
                        "Die Observation konnte nicht gespeichert werden.");
                }

                Cancel();
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync("Fehler beim Speichern", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            SelectedAnimal = null;
            SelectedFeeding = null;
            FeedingBehavior = null;
            Notes = string.Empty;
        }
    }
}