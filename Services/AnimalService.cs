using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.DTOs;
using CrocoManager.Services;

namespace CrocoManager.Services
{
    public class AnimalService : BaseService<AnimalDto>
    {
        public AnimalService(SupabaseClientService supabaseClient)
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
