using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Mappers
{
    public static class AnimalMapper
    {
        public static Animal ToModel(this AnimalDto dto)
        {
            return new Animal
            {
                Id = dto.Id,
                Name = dto.Name,
                Species = dto.Species,
                Gender = dto.Gender ?? string.Empty,
                AgeYears = dto.Age ?? 0,
                Enclosure = dto.Enclosure ?? string.Empty,
                Description = dto.Description
            };
        }

        public static AnimalDto ToDto(this Animal model)
        {
            return new AnimalDto
            {
                Id = model.Id,
                Name = model.Name,
                Species = model.Species,
                Gender = model.Gender,
                Age = model.AgeYears,
                Enclosure = model.Enclosure,
                Description = model.Description
            };
        }
    }
}
