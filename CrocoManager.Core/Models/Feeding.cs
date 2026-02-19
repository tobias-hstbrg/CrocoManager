using CrocoManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Models
{
    public class Feeding
    {
        public Guid Id { get; init; }
        public DateTime FeedingDate { get; init; }
        public string PerformedByEmail { get; init; } = string.Empty;
        public FeedingPlan FeedingPlan { get; init; } = null!;
        public IReadOnlyList<FeedingAnimalStatus> Animals { get; init; } = [];

        public string DisplayName => $"{FeedingDate:dd.MM.yyyy} - {FeedingPlan?.Name ?? "Kein Plan"}";
    }
}
