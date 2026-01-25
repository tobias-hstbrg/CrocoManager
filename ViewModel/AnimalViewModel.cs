using CommunityToolkit.Mvvm.ComponentModel;
using CrocoManager.Models;
using CrocoManager.Services;
using CommunityToolkit.Mvvm.Input;
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
        public ObservableCollection <string> SpeciesOptions { get; }

        public string PageTitle => IsEditing ? "Tier bearbeiten" : "Neues Tier erstellen";

        public AnimalViewModel(AnimalService animalService)
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
            catch(Exception ex)
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
            if(animal == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Löschen bestätigen", $"Möchten Sie '{animal.Name}' wirklich löschen?", "Ja", "Nein");

            if(!confirm) return;

            try
            {
                IsBusy = true;
                await _animalService.DeleteAsync(animal.Id);
                Animals.Remove(animal);

                await ShowSuccessAsync("Tier hinzugefügt", $"'{Name}' wurde erfolgreich hinzugefügt.");
            }
            catch(Exception ex) {
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

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowErrorAsync("Validierungsfehler", "Bitte geben Sie einen Namen ein.").Wait();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                ShowErrorAsync("Validierungsfehler", "Bitte wählen Sie ein Geschlecht aus.").Wait();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Species))
            {
                ShowErrorAsync("Validierungsfehler", "Bitte wählen Sie eine Art aus.").Wait();
                return false;
            }

            if (Age < 0)
            {
                ShowErrorAsync("Validierungsfehler", "Das Alter muss eine positive Zahl sein.").Wait();
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

        private Task ShowSuccessAsync(string title, string message)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        private Task ShowErrorAsync(string title, string message)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }
}
}
