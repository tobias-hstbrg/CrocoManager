using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.Core.DTOs;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Services;

namespace CrocoManager.Core.Services
{
    public class AnimalService : BaseService<AnimalDto>, IAnimalService
    {
        public AnimalService(ISupabaseClientService supabaseClient)
           : base(supabaseClient)
        {
        }

        public async Task<int> GetTotalCount()
        {
            List<AnimalDto> allAnimals = await GetAllAsync();
            return allAnimals.Count;
        }

        public async Task<List<AnimalDto>> GetBySpeciesAsync(string species)
        {
            return await FilterByAsync("species", species);
        }
    }
}
