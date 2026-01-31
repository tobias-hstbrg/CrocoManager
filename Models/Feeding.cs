using CrocoManager.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    public class Feeding
    {
        public Guid Id { get; init; }
        public DateTime FeedingDate { get; init; }
        public string PerformedByEmail { get; init; } = string.Empty;
        public FeedingPlan FeedingPlan { get; init; } = null!;
        public IReadOnlyList<Animal> Animals { get; init; } = [];


    }
}
