using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Models
{
    public class FeedingAnimalStatus
    {
        public Animal Animal { get; init; } = null!;
        public bool WasFed { get; set; }
    }
}
