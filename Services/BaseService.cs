using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public abstract class BaseService<T> where T : BaseModel, new()
    {
        protected readonly SupabaseClientService _supabaseClient;

        public BaseService(SupabaseClientService supabaseClient )
        {
            _supabaseClient = supabaseClient;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            var response = await _supabaseClient.Client.From<T>().Get();
            return response.Models;
        }
    }
}
