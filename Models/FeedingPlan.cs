using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    [Table("feeding_plan")]
    public class FeedingPlan : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("food_type")]
        public string FoodType { get; set; }

        [Column("amount_kg")]
        public double AmountKg { get; set; }

        [Column("frequency_per_week")]
        public int FrequencyPerWeek { get; set; }

        [Column("weekdays")]
        public List<Weekday> Weekdays { get; set; } = [];

        [Column("description")]
        public string Description { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
    public enum Weekday
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
