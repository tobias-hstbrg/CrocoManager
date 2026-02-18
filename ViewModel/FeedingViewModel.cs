using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using CrocoManager.Services;
using System.Collections.ObjectModel;

namespace CrocoManager.ViewModel;

public partial class FeedingViewModel : BaseViewModel
{
    private readonly IFeedingService _feedingService;

    public FeedingViewModel(
        IFeedingService feedingService,
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

    /// <summary>
    /// Defines the permissions a user has for the feeding page
    /// </summary>
    protected override void SetPermissions()
    {
        // Ranger: Create, Read, Update, Delete
        // Scientist: Read (only see todays feeding and history)

        // Not really necessary since these two groups should never be able to see this page by design
        // NotAssigned: Readonly
        // Admin: Create, Read, Update, Delete

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
            Animals.Add(animal);
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
        foreach (var animal in Animals)
        {
            animal.WasFed = false;
        }
    }
}
