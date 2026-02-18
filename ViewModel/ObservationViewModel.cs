using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Mappers;
using CrocoManager.Models;
using CrocoManager.Services;
using System.Collections.ObjectModel;
using static Supabase.Postgrest.Constants;

namespace CrocoManager.ViewModel
{
    public partial class ObservationViewModel : BaseViewModel
    {
        private readonly IObservationService _observationService;
        private readonly IAnimalService _animalService;
        private readonly IFeedingService _feedingService;
        private readonly IFeedingPlanService _feedingPlanService;
        private readonly ISupabaseClientService _supabase;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveObservationCommand))]
        private decimal? airTemperature;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveObservationCommand))]
        private decimal? humidity;

        [ObservableProperty]
        private decimal waterTemperature;

        [ObservableProperty]
        private decimal phValue;

        [ObservableProperty]
        private decimal salinity;

        [ObservableProperty]
        private ObservableCollection<Animal> animals;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveObservationCommand))]
        private Animal? selectedAnimal;

        [ObservableProperty]
        private ObservableCollection<Feeding> feedings;

        [ObservableProperty]
        private ObservableCollection<string> feedingsDisplayNames = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveObservationCommand))]
        private Feeding? selectedFeeding;

        [ObservableProperty]
        private ObservableCollection<string> feedingBehaviors;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveObservationCommand))]
        private string? feedingBehavior;

        [ObservableProperty]
        private string? notes;

        [ObservableProperty]
        private ObservableCollection<Observation> recentObservations;

        public bool CanSave =>
        SelectedAnimal != null &&
        SelectedFeeding != null &&
        !string.IsNullOrWhiteSpace(FeedingBehavior) &&
        AirTemperature.HasValue &&
        Humidity.HasValue;

        public ObservationViewModel(IServiceProvider serviceProvider, ISupabaseClientService supabase, IObservationService observationService, IAnimalService animalService, IFeedingService feedingService, IFeedingPlanService feedingPlanService)
            : base(serviceProvider)
        {
            _observationService = observationService;
            _animalService = animalService;
            _feedingService = feedingService;
            _feedingPlanService = feedingPlanService;
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

        /// <summary>
        /// Defines the permissions a user has for the observation page
        /// </summary>
        protected override void SetPermissions()
        {
            // Ranger: Read
            // Scientist: Create, Read, Update, Delete

            // Not really necessary since these two groups should never be able to see this page by design
            // NotAssigned: Readonly
            // Admin: Create, Read, Update, Delete

            switch (CurrentUserRole)
            {
                case UserRole.Scientist:
                    CanCreate = true;
                    CanEdit = true;
                    CanDelete = true;
                    IsReadOnly = false;
                    CanViewItem = true;
                    break;

                case UserRole.Ranger:
                    CanCreate = false;
                    CanEdit = false;
                    CanDelete = false;
                    IsReadOnly = true;
                    CanViewItem = false;
                    break;

                case UserRole.Admin:
                    CanCreate = true;
                    CanEdit = true;
                    CanDelete = true;
                    IsReadOnly = false;
                    CanViewItem = true;
                    break;

                case UserRole.NotAssigned:
                default:
                    CanCreate = false;
                    CanEdit = false;
                    CanDelete = false;
                    IsReadOnly = true;
                    CanViewItem = false;
                    break;
            }
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
                await LoadObservationHistory();
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
        private async Task LoadObservationHistory()
        {
            try
            {
                IsBusy = true;

                var observationDtos = await _observationService.GetAllAsync();
                var animalDtos = await _animalService.GetAllAsync();
                var feedingDtos = await _feedingService.GetAllAsync();
                var feedingPlanDtos = await _feedingPlanService.GetAllAsync();

                // 1️⃣ EnvironmentalData IDs sammeln
                var envIds = observationDtos
                    .Where(o => o.EnvironmentalDataId.HasValue)
                    .Select(o => o.EnvironmentalDataId!.Value)
                    .Distinct()
                    .ToList();

                Dictionary<Guid, EnvironmentalData> environmentalLookup = new();

                if (envIds.Any())
                {
                    var envResponse = await _supabase.Client
                        .From<EnvironmentalDataDto>()
                        .Filter("id", Supabase.Postgrest.Constants.Operator.In, envIds)
                        .Get();

                    environmentalLookup = envResponse.Models
                        .Select(e => e.ToEntity())
                        .ToDictionary(e => e.Id);
                }

                // 2️⃣ Animal Lookup
                var animals = animalDtos
                    .Select(a => a.ToModel())
                    .ToDictionary(a => a.Id);

                // 3️⃣ FeedingPlan Lookup
                var feedingPlans = feedingPlanDtos
                    .Select(p => p.ToModel())
                    .ToDictionary(p => p.Id);

                // 4️⃣ Feeding Lookup (mit Plan)
                var feedings = feedingDtos
                .Select(f =>
                {
                    var plan = feedingPlans[f.FeedingPlanId];
                    return f.ToModel(plan: plan);
                })
                .ToDictionary(f => f.Id);

                // 5️⃣ Aggregation
                var observations = new List<Observation>();

                foreach (var dto in observationDtos)
                {
                    if (!animals.TryGetValue(dto.AnimalId, out var animal))
                        continue;

                    if (!feedings.TryGetValue(dto.FeedingId, out var feeding))
                        continue;

                    EnvironmentalData? envData = null;

                    if (dto.EnvironmentalDataId.HasValue)
                    {
                        environmentalLookup.TryGetValue(
                            dto.EnvironmentalDataId.Value,
                            out envData);
                    }

                    var observation = dto.ToEntity(animal, feeding, envData);
                    observations.Add(observation);
                }

                var recent = observations
                    .OrderByDescending(o => o.UpdatedAt)
                    .Take(5)
                    .ToList();

                RecentObservations.Clear();

                foreach (var obs in recent)
                {
                    RecentObservations.Add(obs);
                }

                if (!AirTemperature.HasValue || !Humidity.HasValue)
                {
                    await NotificationService.ShowErrorAsync(
                        "Unvollständige Umweltdaten",
                        "Nicht alle benötigten Umweltdaten konnten geladen werden. Eine Speicherung ist derzeit nicht möglich."
                    );
                }
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync(
                    "Fehler beim Laden",
                    ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand(CanExecute = nameof(CanSave))]
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


                if (!AirTemperature.HasValue || !Humidity.HasValue)
                {
                    await NotificationService.ShowErrorAsync(
                        "Speichern nicht möglich",
                        "Nicht alle erforderlichen Umweltdaten sind vorhanden."
                    );
                    return;
                }

                var environmentalDto = new EnvironmentalDataDto
                {
                    MeasurementDate = environmentalData.MeasurementDate,
                    MeasurementTime = environmentalData.MeasurementTime,
                    AirTemperatureCelsius = environmentalData.AirTemperatureCelsius.Value,
                    HumidityPercent = environmentalData.HumidityPercent.Value,
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