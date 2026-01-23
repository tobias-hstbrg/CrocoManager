using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    ///<summary>
    /// Represents an animal managed in the station.
    /// Contains species information, age, enclosure assignment, and physical characteristics.
    /// Used for tracking feeding schedules, observations, and environmental data.
    /// </summary>
    [Table("animals")]
    public class Animal : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("species")]
        public string Species { get; set; } = string.Empty;

        [Column("gender")]
        public string? Gender { get; set; } 

        /// <summary>
        /// Age of the animal in years.
        /// </summary>
        [Column("age_years")]
        public int? Age { get; set; }

        [Column("enclosure")]
        public string? Enclosure { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
