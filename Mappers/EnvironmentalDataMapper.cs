using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Mappers
{
    public static class EnvironmentalDataMapper
    {
        public static EnvironmentalData ToEntity(this EnvironmentalDataDto dto)
        {
            return new EnvironmentalData(
                id: dto.Id,
                measurementDate: dto.MeasurementDate,
                measurementTime: dto.MeasurementTime,
                airTemperatureCelsius: dto.AirTemperatureCelsius,
                humidityPercent: dto.HumidityPercent,
                waterTemperatureCelsius: dto.WaterTemperatureCelsius,
                phValue: dto.PhValue,
                salinityPpt: dto.SalinityPpt
            );
        }

        public static EnvironmentalDataDto ToDto(this EnvironmentalData entity)
        {
            return new EnvironmentalDataDto
            {
                Id = entity.Id,
                MeasurementDate = entity.MeasurementDate,
                MeasurementTime = entity.MeasurementTime,
                AirTemperatureCelsius = entity.AirTemperatureCelsius,
                HumidityPercent = entity.HumidityPercent,
                WaterTemperatureCelsius = entity.WaterTemperatureCelsius,
                PhValue = entity.PhValue,
                SalinityPpt = entity.SalinityPpt
            };
        }
    }
}
