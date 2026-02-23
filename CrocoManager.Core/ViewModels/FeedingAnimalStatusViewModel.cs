using CommunityToolkit.Mvvm.ComponentModel;
using CrocoManager.Core.Models;
using System;

namespace CrocoManager.Core.ViewModels
{
    public partial class FeedingAnimalStatusViewModel : ObservableObject
    {
        private readonly Action _onChanged;
        public FeedingAnimalStatus Status { get; }

        public Animal Animal => Status.Animal;

        [ObservableProperty]
        private bool wasFed;

        public FeedingAnimalStatusViewModel(FeedingAnimalStatus status, Action onChanged)
        {
            Status = status;
            _onChanged = onChanged;
            WasFed = status.WasFed;
        }
        partial void OnWasFedChanged(bool value)
        {
            _onChanged();
        }

        public void ApplyToModel()
        {
            Status.WasFed = WasFed;
        }
    }
}
