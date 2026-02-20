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
        IFeedingService feedingService)
        : base(navigationService, notificationService, authService)
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
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            CurrentFeeding = await _feedingService.GetTodayFeedingDraftAsync();
            HasCurrentFeeding = CurrentFeeding != null;

            LoadAnimals();
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

        IsBusy = true;
        try
        {
            var userEmail = await AuthService.GetUserEmail();

            foreach (var vm in Animals)
                vm.ApplyToModel();

            await _feedingService.SaveFeedingAsync(CurrentFeeding, userEmail);

            await NotificationService.ShowSuccessAsync("Erfolgreich", "Fütterung gespeichert");

            await LoadHistoryAsync();
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
            await NotificationService.ShowErrorAsync("Fehler beim Laden des Verlaufs", ex.Message);
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

