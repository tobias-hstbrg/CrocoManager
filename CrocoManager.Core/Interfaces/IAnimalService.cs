using CrocoManager.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface IAnimalService : IBaseService<AnimalDto>
    {
        Task<int> GetTotalCount();
        Task<List<AnimalDto>> GetBySpeciesAsync(string species);
    }
}
