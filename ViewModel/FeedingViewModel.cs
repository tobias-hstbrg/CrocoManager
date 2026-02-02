using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Models;
using CrocoManager.Services;
using System.Collections.ObjectModel;

namespace CrocoManager.ViewModel;

public partial class FeedingViewModel : BaseViewModel
{
    private readonly FeedingService _feedingService;

    public FeedingViewModel(
        FeedingService feedingService,
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _feedingService = feedingService;
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private Feeding? currentFeeding;

    [ObservableProperty]
    private bool hasHistory;

    [ObservableProperty]
    private bool hasCurrentFeeding;

    public ObservableCollection<FeedingAnimalStatus> Animals { get; } = new();

    public ObservableCollection<FeedingHistoryEntry> FeedingHistory { get; } = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            CurrentFeeding = await _feedingService.GetTodayFeedingDraftAsync();
            HasCurrentFeeding = CurrentFeeding != null;

            Animals.Clear();

            if (CurrentFeeding != null)
            {
                foreach (var animal in CurrentFeeding.Animals)
                    Animals.Add(animal);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (CurrentFeeding == null) return;

        IsBusy = true;
        try
        {
            var userEmail = await AuthService.GetUserEmail();

            await _feedingService.SaveFeedingAsync(
                CurrentFeeding,
                userEmail);

            await NotificationService.ShowSuccessAsync(
                "Erfolgreich",
                "Fütterung gespeichert");

            await LoadHistoryAsync();
            await ClearSelection();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoadHistoryAsync()
    {
        try
        {
            var history = await _feedingService.GetHistoryAsync();
            FeedingHistory.Clear();

            // Sort by date descending (newest first) and take last 10
            foreach (var entry in history.OrderByDescending(h => h.FeedingDate).Take(10))
            {
                FeedingHistory.Add(entry);
            }

            HasHistory = FeedingHistory.Count > 0;
        }
        catch (Exception ex)
        {
            await NotificationService.ShowErrorAsync(
                "Fehler beim Laden des Verlaufs",
                ex.Message);
        }
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await ClearSelection();
    }

    private async Task ClearSelection()
    {
        // Uncheck all animals
        foreach (var animal in Animals)
        {
            animal.WasFed = false;
        }

        // Reload current feeding to reset state
        await LoadAsync();
    }
}
