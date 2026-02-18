using CrocoManager.Core.DTOs;
using CrocoManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface IObservationService : IBaseService<ObservationDto>
    {
        Task<EnvironmentalData> FetchEnvironmentalDataAsync();
    }
}
