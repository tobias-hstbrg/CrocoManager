using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.DTOs;

namespace CrocoManager.Models
{
    public class FeedingPlan
    {

        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string FoodType { get; init; } = string.Empty;
        public decimal AmountKg { get; init; }
        public int FrequencyPerWeek { get; init; }
        public IReadOnlyList<Weekday> Weekdays { get; init; } = [];
        public bool IsActive { get; init; }
    }
}
