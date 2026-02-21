using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Models;
using System.Collections.ObjectModel;

namespace CrocoManager.Core.ViewModels;

public partial class FeedingViewModel : BaseViewModel
{
    private readonly IFeedingService _feedingService;

    public FeedingViewModel(
        INavigationService navigationService,
        INotificationService notificationService,
        IAuthService authService,
        IConnectivityService connectivityService,
        IFeedingService feedingService)
        : base(navigationService, notificationService, authService, connectivityService)
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

    public ObservableCollection<FeedingAnimalStatusViewModel> Animals { get; } = new();

    public ObservableCollection<FeedingHistoryEntry> FeedingHistory { get; } = new();
    public bool HasSelection => Animals.Any(a => a.WasFed);

    /// <summary>
    /// Defines the permissions a user has for the feeding page
    /// </summary>
    protected override void SetPermissions()
    {
        switch(CurrentUserRole)
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

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (!ConnectivityService.IsConnected)
        {
            await DisplayError("Keine Verbindung", new Exception("Netzwerk nicht erreichbar."));
            return;
        }

        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var draftTask = _feedingService.GetTodayFeedingDraftAsync();
            var historyTask = LoadHistoryInternalAsync();

            await Task.WhenAll(draftTask, historyTask);

            CurrentFeeding = draftTask.Result;
            HasCurrentFeeding = CurrentFeeding != null;

            LoadAnimals();
        }
        catch (Exception ex)
        {
            await DisplayError("Fehler beim Laden", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadAnimals()
    {
        Animals.Clear();

        if (CurrentFeeding == null) return;

        foreach (var animal in CurrentFeeding.Animals)
        {
            Animals.Add(new FeedingAnimalStatusViewModel(animal, () => OnPropertyChanged(nameof(HasSelection))));
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (CurrentFeeding == null) return;

        if (!HasSelection)
        {
            await NotificationService.ShowErrorAsync("Fehler", "Bitte wählen Sie mindestens ein Tier aus.");
            return;
        }

        IsBusy = true;
        try
        {
            var userEmail = await AuthService.GetUserEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                await NotificationService.ShowErrorAsync("Fehler", "Benutzer-E-Mail konnte nicht ermittelt werden.");
                return;
            }

            foreach (var vm in Animals)
                vm.ApplyToModel();

            await _feedingService.SaveFeedingAsync(CurrentFeeding, userEmail);

            await NotificationService.ShowSuccessAsync("Erfolgreich", "Fütterung gespeichert");

            await LoadHistoryInternalAsync();
            ClearSelection();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoadHistoryAsync()
    {
        if (IsBusy) return;
        await LoadHistoryInternalAsync();
    }

    private async Task LoadHistoryInternalAsync()
    {
        IsBusy = true;
        try
        {
            var history = await _feedingService.GetHistoryAsync();
            FeedingHistory.Clear();

            foreach (var entry in history.OrderByDescending(h => h.FeedingDate).Take(10))
            {
                FeedingHistory.Add(entry);
            }

            HasHistory = FeedingHistory.Count > 0;
        }
        catch (Exception ex)
        {
            await DisplayError("Fehler beim Laden des Verlaufs", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        ClearSelection();
    }

    private void ClearSelection()
    {
        foreach (var vm in Animals)
            vm.WasFed = false;
        OnPropertyChanged(nameof(HasSelection));
    }
}

