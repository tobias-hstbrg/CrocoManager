using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Models
{
    public class Observation
    {
        public Guid Id { get; init; }
        public Animal Animal { get; init; } = null!;

        public Feeding Feeding { get; init; } = null!;
        public EnvironmentalData? EnvironmentalData { get; init; }
        public string? FeedingBehavior { get; init; }
        public string? Notes { get; init; }
        public string ResearcherEmail { get; init; } = string.Empty;
        public DateTime UpdatedAt { get; init; }
        public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

        public bool HasEnvironmentalData => EnvironmentalData != null;
    }
}
