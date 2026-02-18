using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrocoManager.Interfaces;
using CrocoManager.Services;
using CrocoManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
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
        private string activePlanDescription;

        [ObservableProperty]
        private DateTime? lastFeedingDate;

        [ObservableProperty]
        private string lastFeedingPlanName = "-";

        [ObservableProperty]
        private string lastFeedingStatus = "-";

        public HomeViewModel(IServiceProvider serviceProvider, IAnimalService animalService, IFeedingPlanService feedingPlanService, IFeedingService feedingService) : base(serviceProvider)
        {
            _animalService = animalService;
            _feedingService = feedingService;
            _feedingPlanService = feedingPlanService;
            _ = LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            try
            {
                IsBusy = true;

                TotalAnimals = await _animalService.GetTotalCount();
                TotalFeedingPlans = await _feedingPlanService.GetTotalCount();
                FeedingsThisWeek = await _feedingService.GetCurrentWeekCount();

                var activeFeedingPlan = await _feedingPlanService.GetActivePlanAsync();

                if (activeFeedingPlan != null)
                {
                    ActivePlanName = activeFeedingPlan.Name;
                    ActivePlanFood = activeFeedingPlan.FoodType;
                    ActivePlanFrequency =  activeFeedingPlan.FrequencyPerWeek + "x pro Woche (" + activeFeedingPlan.WeekdaysFormatted + ")";
                    ActivePlanDescription = activeFeedingPlan.Description;
                }

                var latest = (await _feedingService.GetHistoryAsync()).MaxBy(Entry => Entry.FeedingDate);
                if (latest != null)
                {
                    LastFeedingDate = latest != null ? latest.FeedingDate : null;
                    LastFeedingPlanName = latest != null ? latest.FeedingPlanName : "Bisher keine Fütterung";
                    LastFeedingStatus = latest != null ? $"{latest.FedAnimals} von {latest.TotalAnimals} gefüttert" : "Bisher keine Fütterung";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
