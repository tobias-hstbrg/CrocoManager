using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Mappers
{
    public static class ObservationMapper
    {
        public static Observation ToEntity(this ObservationDto dto, Animal animal, Feeding feeding, EnvironmentalData? environmentalData)
        {
            return new Observation
            {
                Id = dto.Id,
                Animal = animal,
                Feeding = feeding,
                EnvironmentalData = environmentalData,
                FeedingBehavior = dto.FeedingBehavior,
                Notes = dto.Notes,
                ResearcherEmail = dto.ResearcherName ?? string.Empty
            };
        }

        public static ObservationDto ToDto(this Observation entity)
        {
            return new ObservationDto
            {
                Id = entity.Id,
                AnimalId = entity.Animal.Id,
                FeedingId = entity.Feeding.Id,
                EnvironmentalDataId = entity.EnvironmentalData?.Id,
                FeedingBehavior = entity.FeedingBehavior,
                Notes = entity.Notes,
                ResearcherName = entity.ResearcherEmail
            };
        }
    }
}
