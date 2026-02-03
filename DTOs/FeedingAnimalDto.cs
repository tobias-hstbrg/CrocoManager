using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.DTOs
{
    [Table("feeding_animals")]
    public class FeedingAnimalDto : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("feeding_id")]
        public Guid FeedingId { get; set; }

        [Column("animal_id")]
        public Guid AnimalId { get; set; }

        [Column("was_fed")]
        public bool WasFed { get; set; }
    }
}
