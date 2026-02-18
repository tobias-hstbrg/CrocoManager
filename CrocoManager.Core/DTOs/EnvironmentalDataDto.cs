using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.DTOs
{
    [Table("environmental_data")]
    public class EnvironmentalDataDto : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("measurement_date")]
        public DateOnly MeasurementDate { get; set; }

        [Column("measurement_time")]
        public TimeSpan MeasurementTime { get; set; }

        [Column("air_temperature_celsius")]
        public decimal AirTemperatureCelsius { get; set; }

        [Column("humidity_percent")]
        public decimal HumidityPercent { get; set; }

        [Column("water_temperature_celsius")]
        public decimal WaterTemperatureCelsius { get; set; }

        [Column("ph_value")]
        public decimal PhValue { get; set; }

        [Column("salinity_ppt")]
        public decimal SalinityPpt { get; set; }
    }
}
