using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IAnimalService _animalService;
        private readonly IFeedingPlanService _feedingPlanService;
        private readonly IFeedingService _feedingService;

        [ObservableProperty]
        private int totalAnimals;

        [ObservableProperty]
        private int totalFeedingPlans;

        [ObservableProperty]
        private int feedingsThisWeek;

        [ObservableProperty]
        private string activePlanName = "-";

        [ObservableProperty]
        private string activePlanFood = "-";

        [ObservableProperty]
        private string activePlanFrequency = "-";

        [ObservableProperty]
        private string? activePlanDescription;

        [ObservableProperty]
        private DateTime? lastFeedingDate;

        [ObservableProperty]
        private string lastFeedingPlanName = "-";

        [ObservableProperty]
        private string lastFeedingStatus = "-";

        public HomeViewModel(
            INavigationService navigationService,
            INotificationService notificationService,
            IAuthService authService,
            IConnectivityService connectivityService,
            IAnimalService animalService, 
            IFeedingPlanService feedingPlanService, 
            IFeedingService feedingService) 
            : base(navigationService, notificationService, authService, connectivityService)
        {
            _animalService = animalService;
            _feedingService = feedingService;
            _feedingPlanService = feedingPlanService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (!ConnectivityService.IsConnected)
            {
                await DisplayError("Keine Verbindung", new Exception("Netzwerk nicht erreichbar."));
                return;
            }

            try
            {
                IsBusy = true;

                // load data in parallel to fetch data faster
                var totalAnimalsTask = _animalService.GetTotalCount();
                var totalPlansTask = _feedingPlanService.GetTotalCount();
                var currentWeekCountTask = _feedingService.GetCurrentWeekCount();
                var activePlanTask = _feedingPlanService.GetActivePlanAsync();
                var historyTask = _feedingService.GetHistoryAsync();

                await Task.WhenAll(totalAnimalsTask, totalPlansTask, currentWeekCountTask, activePlanTask, historyTask);

                TotalAnimals = totalAnimalsTask.Result;
                TotalFeedingPlans = totalPlansTask.Result;
                FeedingsThisWeek = currentWeekCountTask.Result;

                var activeFeedingPlan = activePlanTask.Result;
                if (activeFeedingPlan != null)
                {
                    ActivePlanName = activeFeedingPlan.Name;
                    ActivePlanFood = activeFeedingPlan.FoodType;
                    ActivePlanFrequency =  activeFeedingPlan.FrequencyPerWeek + "x pro Woche (" + activeFeedingPlan.WeekdaysFormatted + ")";
                    ActivePlanDescription = activeFeedingPlan.Description;
                }

                var latest = historyTask.Result.MaxBy(Entry => Entry.FeedingDate);
                if (latest != null)
                {
                    LastFeedingDate = latest.FeedingDate;
                    LastFeedingPlanName = latest.FeedingPlanName;
                    LastFeedingStatus = $"{latest.FedAnimals} von {latest.TotalAnimals} gefüttert";
                }
            }
            catch (Exception ex)
            {
                await DisplayError("Fehler beim Laden des Dashboards", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
