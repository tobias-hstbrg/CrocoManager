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
        public FeedingAnimalStatus Status { get; }

        public Animal Animal => Status.Animal;

        [ObservableProperty]
        private bool wasFed;

        public FeedingAnimalStatusViewModel(FeedingAnimalStatus status)
        {
            Status = status;
            WasFed = status.WasFed;
        }

        public void ApplyToModel()
        {
            Status.WasFed = WasFed;
        }
    }
}
