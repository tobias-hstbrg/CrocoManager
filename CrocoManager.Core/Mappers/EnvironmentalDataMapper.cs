using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Mappers
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
            if (!entity.AirTemperatureCelsius.HasValue || !entity.HumidityPercent.HasValue)
            {
                throw new InvalidOperationException(
                    "EnvironmentalData ist unvollständig und kann nicht persistiert werden."
                );
            }

            return new EnvironmentalDataDto
            {
                Id = entity.Id,
                MeasurementDate = entity.MeasurementDate,
                MeasurementTime = entity.MeasurementTime,
                AirTemperatureCelsius = entity.AirTemperatureCelsius.Value,
                HumidityPercent = entity.HumidityPercent.Value,
                WaterTemperatureCelsius = entity.WaterTemperatureCelsius,
                PhValue = entity.PhValue,
                SalinityPpt = entity.SalinityPpt
            };
        }
    }
}
