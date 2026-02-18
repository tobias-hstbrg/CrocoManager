using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Models;
using CrocoManager.Interfaces;
using CrocoManager.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.Mappers;
using CrocoManager.Services;

namespace CrocoManager.ViewModel
{
    public partial class AnimalViewModel : BaseViewModel
    {
        private readonly IAnimalService _animalService;

        [ObservableProperty]
        private ObservableCollection<Animal> animals;

        [ObservableProperty]
        private Animal? selectedAnimal;

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

        public AnimalViewModel(IServiceProvider serviceProvider, IAnimalService animalService) : base(serviceProvider)
        {
            _animalService = animalService;

            Animals = new ObservableCollection<Animal>();

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

        /// <summary>
        /// Defines the permissions a user has for the animal page
        /// </summary>
        protected override void SetPermissions()
        {
            // Ranger: Create, Read, Update, Delete
            // Scientist: Read

            // Not really necessary since these two groups should never be able to see this page by design
            // NotAssigned: Readonly
            // Admin: Create, Read, Update, Delete

            switch (CurrentUserRole)
            {
                case UserRole.Scientist:
                    CanCreate = false;
                    CanEdit = false;
                    CanDelete = false;
                    IsReadOnly = true;
                    CanViewItem = false;
                    break;

                case UserRole.Ranger:
                    CanCreate = true;
                    CanEdit = true;
                    CanDelete = true;
                    IsReadOnly = false;
                    CanViewItem = true;
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
                var dtos = await _animalService.GetAllAsync();

                Animals.Clear();
                foreach (var dto in dtos)
                {
                    Animals.Add(dto.ToModel());
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
            if (!CanCreate) return;

            IsEditing = true;
            ClearForm();
        }

        [RelayCommand]
        private void EditAnimal(Animal animal)
        {
            // Prüfung direkt in der Methode
            if (!CanEdit || animal == null) return;

            IsEditing = true;
            SelectedAnimal = animal;

            Name = animal.Name;
            Gender = animal.Gender;
            Age = animal.AgeYears;
            Species = animal.Species;
            Enclosure = animal.Enclosure;
            Description = animal.Description;
        }

        [RelayCommand]
        private async Task DeleteAnimal(Animal animal)
        {
            if (!CanDelete || animal == null) return;

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

        private bool CanSaveAnimal()
        {
            if (IsBusy) return false;

            // Beim Bearbeiten: CanEdit erforderlich
            // Beim Erstellen: CanCreate erforderlich
            return IsEditing ? CanEdit : CanCreate;
        }

        [RelayCommand]
        private async Task SaveAnimal()
        {
            if(!CanSaveAnimal()) return;

            if (!await ValidateForm())
            {
                return;
            }

            try
            {
                IsBusy = true;

                if (IsEditing && SelectedAnimal != null)
                {
                    var updatedAnimal = new Animal
                    {
                        Id = SelectedAnimal.Id,
                        Name = Name,
                        Gender = Gender,
                        AgeYears = Age,
                        Species = Species,
                        Enclosure = Enclosure,
                        Description = Description
                    };

                    var updatedDto = await _animalService.UpdateAsync(updatedAnimal.ToDto());

                    if(updatedDto != null)
                    {
                        var index = Animals.IndexOf(SelectedAnimal);
                        if (index >= 0)
                        {
                            Animals[index] = updatedAnimal;
                        }

                        await NotificationService.ShowSuccessAsync(
                            "Tier aktualisiert",
                            $"'{Name}' wurde erfolgreich aktualisiert.");
                    }
                    else
                    {
                        await NotificationService.ShowErrorAsync(
                            "Fehler",
                            "Das Tier konnte nicht aktualisiert werden.");
                    }
                }
                else
                {
                    // Add new animal
                    var newAnimal = new Animal
                    {
                        Name = Name,
                        Gender = Gender,
                        AgeYears = Age,
                        Species = Species,
                        Enclosure = Enclosure,
                        Description = Description
                    };

                    var createdDto = await _animalService.AddAsync(newAnimal.ToDto());

                    if (createdDto != null)
                    {
                        Animals.Add(createdDto.ToModel());
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
