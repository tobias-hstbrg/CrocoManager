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

    public ObservableCollection<FeedingAnimalStatus> Animals { get; }
        = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            CurrentFeeding = await _feedingService.GetTodayFeedingDraftAsync();

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
        }
        finally
        {
            IsBusy = false;
        }
    }
}
