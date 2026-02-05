using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Models;
using CrocoManager.Services;
using System.Collections.ObjectModel;

namespace CrocoManager.ViewModel
{
    public partial class ObservationViewModel : BaseViewModel
    {
        private readonly ObservationService _observationService;
        private readonly AnimalService _animalService;

        [ObservableProperty] private decimal airTemperature;
        [ObservableProperty] private decimal humidity;
        [ObservableProperty] private decimal waterTemperature;
        [ObservableProperty] private decimal phValue;
        [ObservableProperty] private decimal salinity;

        [ObservableProperty] private ObservableCollection<Animal> animals;
        [ObservableProperty] private Animal? selectedAnimal;

        [ObservableProperty] private ObservableCollection<Feeding> feedings;
        [ObservableProperty] private Feeding? selectedFeeding;

        [ObservableProperty] private ObservableCollection<string> feedingBehaviors;
        [ObservableProperty] private string? feedingBehavior;
        [ObservableProperty] private string? notes;

        public ObservationViewModel( IServiceProvider serviceProvider, ObservationService observationService, AnimalService animalService)
            : base(serviceProvider)
        {
            _observationService = observationService;
            _animalService = animalService;

            Animals = new();
            Feedings = new();
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

                var animals = await _animalService.GetAllAsync();
                Animals.Clear();
                foreach (var a in animals)
                    Animals.Add(a);
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync(
                    "Fehler beim Laden",
                    ex.Message);
            }
        }

        [RelayCommand]
        private async Task SaveObservation()
        {
            // wired later to CreateObservationAsync
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