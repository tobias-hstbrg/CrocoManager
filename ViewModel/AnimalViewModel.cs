using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class AnimalViewModel : BaseViewModel
    {
        private readonly AnimalService _animalService;

        [ObservableProperty]
        private ObservableCollection<AnimalDto> animals;

        [ObservableProperty]
        private AnimalDto selectedAnimal;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string gender;

        [ObservableProperty]
        private int age;

        [ObservableProperty]
        private string species;

        [ObservableProperty]
        private string enclosure;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private bool isBusy;

        public ObservableCollection<string> GenderOptions { get; }
        public ObservableCollection<string> SpeciesOptions { get; }

        public string PageTitle => IsEditing ? "Tier bearbeiten" : "Neues Tier erstellen";

        public AnimalViewModel(IServiceProvider serviceProvider, AnimalService animalService) : base(serviceProvider)
        {
            _animalService = animalService;

            Animals = new ObservableCollection<AnimalDto>();

            GenderOptions = new ObservableCollection<string>()
            {
                "Männlich",
                "Weiblich"
            };

            SpeciesOptions = new ObservableCollection<string>()
            {
                "Amerikanischer Alligator",
                "Mississippi-Alligator",
                "Spitzkrokodil",
                "Amerikanisches Krokodil"
            };

            ClearForm();
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(PageTitle));
        }

        [RelayCommand]
        private async Task LoadAnimalsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var animals = await _animalService.GetAllAsync();

                Animals.Clear();
                foreach (var animal in animals)
                {
                    Animals.Add(animal);
                }
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync("Fehler beim Laden", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void AddNewAnimal()
        {
            IsEditing = true;
            ClearForm();
        }

        [RelayCommand]
        private void EditAnimal(AnimalDto animal)
        {
            if (animal == null) return;

            IsEditing = true;
            SelectedAnimal = animal;

            Name = animal.Name;
            Gender = animal.Gender;
            Age = animal.Age.Value;
            Species = animal.Species;
            Enclosure = animal.Enclosure;
            Description = animal.Description;
        }

        [RelayCommand]
        private async Task DeleteAnimal(AnimalDto animal)
        {
            if (animal == null) return;

            bool confirm = await NotificationService.ShowConfirmationAsync("Löschen bestätigen", $"Möchten Sie '{animal.Name}' wirklich löschen?", "Ja", "Nein");

            if (!confirm) return;

            try
            {
                string animalName = animal.Name;
                IsBusy = true;
                await _animalService.DeleteAsync(animal.Id);
                Animals.Remove(animal);

                await NotificationService.ShowSuccessAsync("Tier gelöscht", $"'{animalName}' wurde erfolgreich gelöscht.");
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
        private async Task SaveAnimal()
        {
            if (!await ValidateForm())
            {
                return;
            }

            try
            {
                IsBusy = true;

                if (IsEditing && SelectedAnimal != null)
                {
                    // Update existing animal
                    SelectedAnimal.Name = Name;
                    SelectedAnimal.Gender = Gender;
                    SelectedAnimal.Age = Age;
                    SelectedAnimal.Species = Species;
                    SelectedAnimal.Enclosure = Enclosure;
                    SelectedAnimal.Description = Description;

                    var updatedAnimal = await _animalService.UpdateAsync(SelectedAnimal);

                    if (updatedAnimal != null)
                    {
                        await NotificationService.ShowSuccessAsync("Tier aktualisiert", $"'{Name}' wurde erfolgreich aktualisiert.");
                    }
                    else
                    {
                        await NotificationService.ShowErrorAsync("Fehler", "Das Tier konnte nicht aktualisiert werden.");
                    }
                }
                else
                {
                    // Add new animal
                    var newAnimal = new AnimalDto
                    {
                        Name = Name,
                        Gender = Gender,
                        Age = Age,
                        Species = Species,
                        Enclosure = Enclosure,
                        Description = Description
                    };

                    var createdAnimal = await _animalService.AddAsync(newAnimal);

                    if (createdAnimal != null)
                    {
                        Animals.Add(createdAnimal);
                        await NotificationService.ShowSuccessAsync("Tier hinzugefügt", $"'{Name}' wurde erfolgreich hinzugefügt.");
                    }
                    else
                    {
                        await NotificationService.ShowErrorAsync("Fehler", "Das Tier konnte nicht hinzugefügt werden.");
                    }
                }

                ClearForm();
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
            ClearForm();
        }

        private async Task<bool> ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Bitte geben Sie einen Namen ein.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Bitte wählen Sie ein Geschlecht aus.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Species))
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Bitte wählen Sie eine Art aus.");
                return false;
            }

            if (Age < 0)
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Das Alter muss eine positive Zahl sein.");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Gender = "Männlich";
            Age = 0;
            Species = string.Empty;
            Enclosure = string.Empty;
            Description = string.Empty;
            SelectedAnimal = null;
            IsEditing = false;
        }
    }
}
