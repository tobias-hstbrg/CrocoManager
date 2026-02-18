using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Interfaces
{
    public interface IObservationService : IBaseService<ObservationDto>
    {
        Task<EnvironmentalData> FetchEnvironmentalDataAsync();
    }
}
