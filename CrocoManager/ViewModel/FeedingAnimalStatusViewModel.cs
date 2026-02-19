using CommunityToolkit.Mvvm.ComponentModel;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.ViewModel
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
