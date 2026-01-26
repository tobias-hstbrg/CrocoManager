using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Models;
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
    public partial class AnimalViewModel : ObservableObject
    {
        private readonly AnimalService _animalService;
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        [ObservableProperty]
        private ObservableCollection<Animal> animals;

        [ObservableProperty]
        private Animal selectedAnimal;

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

        public AnimalViewModel(IAuthService authService, IServiceProvider serviceProvider, AnimalService animalService)
        {
            _animalService = animalService;
            _authService = authService;
            _serviceProvider = serviceProvider;

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

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(PageTitle));
        }

        [RelayCommand]
        private async Task LoadAnimals()
        {
            if (isBusy) return;

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
                await ShowErrorAsync("Fehler beim Laden", ex.Message);
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
        private void EditAnimal(Animal animal)
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
        private async Task DeleteAnimal(Animal animal)
        {
            if (animal == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Löschen bestätigen", $"Möchten Sie '{animal.Name}' wirklich löschen?", "Ja", "Nein");

            if (!confirm) return;

            try
            {
                string animalName = animal.Name;
                IsBusy = true;
                await _animalService.DeleteAsync(animal.Id);
                Animals.Remove(animal);

                await ShowSuccessAsync("Tier gelöscht", $"'{animalName}' wurde erfolgreich gelöscht.");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Fehler beim Speichern", ex.Message);
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
                        await ShowSuccessAsync("Tier aktualisiert", $"'{Name}' wurde erfolgreich aktualisiert.");
                    }
                    else
                    {
                        await ShowErrorAsync("Fehler", "Das Tier konnte nicht aktualisiert werden.");
                    }
                }
                else
                {
                    // Add new animal
                    var newAnimal = new Animal
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
                        await ShowSuccessAsync("Tier hinzugefügt", $"'{Name}' wurde erfolgreich hinzugefügt.");
                    }
                    else
                    {
                        await ShowErrorAsync("Fehler", "Das Tier konnte nicht hinzugefügt werden.");
                    }
                }

                ClearForm();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Fehler beim Speichern", ex.Message);
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
                await ShowErrorAsync("Validierungsfehler", "Bitte geben Sie einen Namen ein.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                await ShowErrorAsync("Validierungsfehler", "Bitte wählen Sie ein Geschlecht aus.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Species))
            {
                await ShowErrorAsync("Validierungsfehler", "Bitte wählen Sie eine Art aus.");
                return false;
            }

            if (Age < 0)
            {
                await ShowErrorAsync("Validierungsfehler", "Das Alter muss eine positive Zahl sein.");
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

        private async Task ShowSuccessAsync(string title, string message)
        {
            await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
        }

        private async Task ShowErrorAsync(string title, string message)
        {
            await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
        }

        [RelayCommand]
        private async Task SignOut()
        {
            try
            {
                bool succesful = await _authService.SignOutAsync();
                if (succesful)
                {
                    var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                    if (Application.Current?.Windows?.FirstOrDefault() is Window window)
                    {
                        window.Page = loginPage;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Fehler", "Abmeldung fehlgeschlagen. Bitte versuche es erneut.", "Ok");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign out failed: {ex.Message}");
            }
        }
    }
}
