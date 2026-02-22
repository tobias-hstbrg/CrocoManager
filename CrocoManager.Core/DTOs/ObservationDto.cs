using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.DTOs
{
    [Table("observations")]
    public class ObservationDto : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("animal_id")]
        public Guid AnimalId { get; set; }

        [Column("feeding_id")]
        public Guid FeedingId { get; set; }

        [Column("environmental_data_id")]
        public Guid? EnvironmentalDataId { get; set; }

        [Column("feeding_behavior")]
        public string? FeedingBehavior { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("researcher_email")]
        public string? ResearcherEmail { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
