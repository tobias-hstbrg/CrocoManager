using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Models
{
    public class FeedingAnimalItem
    {
        public Animal Animal { get; }
        public bool WasFed
        {
            get => _wasFed;
            set
            {
                _wasFed = value;
                WasFedChanged?.Invoke();
            }
        }

        private bool _wasFed;

        public event Action? WasFedChanged;

        public FeedingAnimalItem(FeedingAnimalStatus status)
        {
            Animal = status.Animal;
            _wasFed = status.WasFed;
        }
    }
}
