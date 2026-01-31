using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    public class Animal
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Species { get; init; } = string.Empty;
        public string Gender { get; init; } = string.Empty;
        public int AgeYears { get; init; }
        public string Enclosure { get; init; } = string.Empty;
        public string? Description { get; init; }

        // UI-friendly, domain-owned
        public string DisplayName =>
            $"{Name} • {Species} • {Enclosure} • {AgeYears} Jahre";
    }
}
