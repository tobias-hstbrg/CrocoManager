using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Models
{
    public class FeedingHistoryEntry
    {
        public DateTime FeedingDate { get; set; }

        public string FeedingPlanName { get; set; } = string.Empty;

        public int FedAnimals { get; set; }

        public int TotalAnimals { get; set; }

        public string PerformedByEmail { get; set; } = string.Empty;

        public string FedRatio => $"{FedAnimals}/{TotalAnimals}";
    }
}