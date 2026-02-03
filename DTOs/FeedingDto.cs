using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.DTOs
{
    [Table ("feedings")]
    public class FeedingDto : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("feeding_plan_id")]
        public Guid FeedingPlanId { get; set; }

        [Column("feeding_date")]
        public DateTime FeedingDate { get; set; }

        [Column("performed_by_email")]
        public string PerformedByEmail { get; set; } = string.Empty;
    }
}
