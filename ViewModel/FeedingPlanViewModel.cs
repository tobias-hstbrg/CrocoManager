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
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
{
    public partial class FeedingPlanViewModel : BaseViewModel
    {
        private readonly FeedingPlanService _feedingPlanService;

        [ObservableProperty]
        private ObservableCollection<FeedingPlanDto> feedingPlans;

        [ObservableProperty]
        private FeedingPlanDto? selectedPlan;

        [ObservableProperty]
        private FeedingPlanDto? activePlan;

        [ObservableProperty]
        private bool hasActivePlan;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string foodType;

        [ObservableProperty]
        private double amountKg;

        [ObservableProperty]
        private int frequencyPerWeek;

        [ObservableProperty]
        private string weekdays;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private bool isBusy;

        public string PageTitle => IsEditing ? "Plan bearbeiten" : "Neuen Plan erstellen";

        public FeedingPlanViewModel(FeedingPlanService feedingPlanService, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _feedingPlanService = feedingPlanService;
            FeedingPlans = new ObservableCollection<FeedingPlanDto>();
            ClearForm();
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(PageTitle));
        }

        [RelayCommand]
        private async Task LoadPlans()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var plans = await _feedingPlanService.GetAllAsync();

                FeedingPlans.Clear();
                foreach (var plan in plans)
                {
                    FeedingPlans.Add(plan);
                }

                ActivePlan = FeedingPlans.FirstOrDefault(p => p.IsActive);
                HasActivePlan = ActivePlan != null;
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
        private void AddNewPlan()
        {
            IsEditing = true;
            ClearForm();
        }

        [RelayCommand]
        private void EditPlan(FeedingPlanDto plan)
        {
            if (plan == null) return;

            IsEditing = true;
            SelectedPlan = plan;

            Name = plan.Name;
            FoodType = plan.FoodType;
            AmountKg = plan.AmountKg;
            FrequencyPerWeek = plan.FrequencyPerWeek;
            Description = plan.Description;

            if (plan.Weekdays != null && plan.Weekdays.Any())
            {
                Weekdays = string.Join(", ", plan.Weekdays.Select(w => w.ToString()));
            }
            else
            {
                Weekdays = string.Empty;
            }
        }

        [RelayCommand]
        private async Task DeletePlan(FeedingPlanDto plan)
        {
            if (plan == null) return;

            bool confirm = await NotificationService.ShowConfirmationAsync(
                "Löschen bestätigen",
                $"Möchten Sie '{plan.Name}' wirklich löschen?",
                "Ja",
                "Nein");

            if (!confirm) return;

            try
            {
                string planName = plan.Name;
                IsBusy = true;
                await _feedingPlanService.DeleteAsync(plan.Id);
                FeedingPlans.Remove(plan);

                if (plan == ActivePlan)
                {
                    ActivePlan = null;
                    HasActivePlan = false;
                }

                await NotificationService.ShowSuccessAsync("Plan gelöscht", $"'{planName}' wurde erfolgreich gelöscht.");
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync("Fehler beim Löschen", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ToggleActivePlan(FeedingPlanDto plan)
        {
            if (plan == null) return;

            try
            {
                IsBusy = true;

                // deactivate all plans
                foreach (var p in FeedingPlans)
                {
                    if (p.IsActive && p.Id != plan.Id)
                    {
                        p.IsActive = false;
                        await _feedingPlanService.UpdateAsync(p);
                    }
                }

                // activate selected plan
                plan.IsActive = true;
                await _feedingPlanService.UpdateAsync(plan);

                // Update UI
                ActivePlan = plan;
                HasActivePlan = true;

                // Force UI Refresh
                var tempList = FeedingPlans.ToList();
                FeedingPlans.Clear();
                foreach (var p in tempList)
                {
                    FeedingPlans.Add(p);
                }

                await NotificationService.ShowSuccessAsync("Plan aktiviert", $"'{plan.Name}' ist jetzt der aktive Plan.");
            }
            catch (Exception ex)
            {
                await NotificationService.ShowErrorAsync("Fehler beim Aktivieren", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SavePlan()
        {
            if (!await ValidateForm())
            {
                return;
            }

            try
            {
                IsBusy = true;

                var weekdaysList = Weekdays
               .Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(d => Enum.Parse<Weekday>(d.Trim(), true))
               .Distinct()
               .ToList();

                if (IsEditing && SelectedPlan != null)
                {
                    // Update existing plan
                    SelectedPlan.Name = Name;
                    SelectedPlan.FoodType = FoodType;
                    SelectedPlan.AmountKg = AmountKg;
                    SelectedPlan.FrequencyPerWeek = FrequencyPerWeek;
                    SelectedPlan.Description = Description;
                    SelectedPlan.Weekdays = weekdaysList;

                    var updatedPlan = await _feedingPlanService.UpdateAsync(SelectedPlan);

                    if (updatedPlan == null)
                    {
                        await NotificationService.ShowErrorAsync(
                            "Fehler",
                            "Der Plan konnte nicht aktualisiert werden."
                        );
                        return;
                    }

                    var index = FeedingPlans.IndexOf(SelectedPlan);
                    if (index >= 0)
                        FeedingPlans[index] = updatedPlan;

                    await NotificationService.ShowSuccessAsync(
                        "Plan aktualisiert",
                        $"'{updatedPlan.Name}' wurde erfolgreich aktualisiert."
                    );
                }
                else
                {
                    // Add new plan
                    var newPlan = new FeedingPlanDto
                    {
                        Id = Guid.NewGuid(),
                        Name = Name,
                        FoodType = FoodType,
                        AmountKg = AmountKg,
                        FrequencyPerWeek = FrequencyPerWeek,
                        Description = Description,
                        IsActive = false,
                        Weekdays = weekdaysList
                    };

                    var createdPlan = await _feedingPlanService.AddAsync(newPlan);

                    if (createdPlan != null)
                    {
                        FeedingPlans.Add(createdPlan);
                        await NotificationService.ShowSuccessAsync("Plan hinzugefügt", $"'{Name}' wurde erfolgreich hinzugefügt.");
                    }
                    else
                    {
                        await NotificationService.ShowErrorAsync("Fehler", "Der Plan konnte nicht hinzugefügt werden.");
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

            if (string.IsNullOrWhiteSpace(FoodType))
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Bitte geben Sie eine Futterart ein.");
                return false;
            }

            if (AmountKg <= 0)
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Die Futtermenge muss größer als 0 sein.");
                return false;
            }

            if (FrequencyPerWeek <= 0)
            {
                await NotificationService.ShowErrorAsync("Validierungsfehler", "Die Frequenz muss größer als 0 sein.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Weekdays))
            {
                await NotificationService.ShowErrorAsync(
                    "Validierungsfehler",
                    "Bitte geben Sie mindestens einen Wochentag an."
                );
                return false;
            }

            var entries = Weekdays.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var trimmed = entry.Trim();

                if (!Enum.TryParse<Weekday>(trimmed, true, out _))
                {
                    await NotificationService.ShowErrorAsync(
                        "Ungültiger Wochentag",
                        $"'{trimmed}' ist kein gültiger Wochentag.\n" +
                        $"Erlaubt sind: {string.Join(", ", Enum.GetNames(typeof(Weekday)))}"
                    );
                    return false;
                }
            }

            return true;
        }

        private void ClearForm()
        {
            Name = string.Empty;
            FoodType = string.Empty;
            AmountKg = 0;
            FrequencyPerWeek = 0;
            Weekdays = string.Empty;
            Description = string.Empty;
            SelectedPlan = null;
            IsEditing = false;
        }
    }
}