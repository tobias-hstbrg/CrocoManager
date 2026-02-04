using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    public class EnvironmentalData
    {
        public Guid Id { get; init; }
        public DateOnly MeasurementDate { get; }
        public TimeSpan MeasurementTime { get; }
        public decimal AirTemperatureCelsius { get; }
        public decimal HumidityPercent { get; }
        public decimal WaterTemperatureCelsius { get; }
        public decimal PhValue { get; }
        public decimal SalinityPpt { get; }
        
        public EnvironmentalData(
            Guid id,
            DateOnly measurementDate,
            TimeSpan measurementTime,
            decimal airTemperatureCelsius,
            decimal humidityPercent,
            decimal waterTemperatureCelsius,
            decimal phValue,
            decimal salinityPpt)
        {
            Id = Guid.Empty;
            MeasurementDate = measurementDate;
            MeasurementTime = measurementTime;
            AirTemperatureCelsius = airTemperatureCelsius;
            HumidityPercent = humidityPercent;
            WaterTemperatureCelsius = waterTemperatureCelsius;
            PhValue = phValue;
            SalinityPpt = salinityPpt;
        }

        public EnvironmentalData(
        DateOnly measurementDate,
        TimeSpan measurementTime,
        decimal airTemperatureCelsius,
        decimal humidityPercent,
        decimal waterTemperatureCelsius,
        decimal phValue,
        decimal salinityPpt)
        : this(Guid.Empty, measurementDate, measurementTime, airTemperatureCelsius, humidityPercent, waterTemperatureCelsius, phValue, salinityPpt)
        {
        }
    }
}
