using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.Interfaces;

namespace CrocoManager.Services
{
    public abstract class BaseService<T> : IBaseService<T> where T : BaseModel, new()
    {
        protected readonly ISupabaseClientService _supabaseClient;

        public BaseService(ISupabaseClientService supabaseClient)
         {
              _supabaseClient = supabaseClient;
           }

        public virtual async Task<List<T>> GetAllAsync()
        {
            var response = await _supabaseClient.Client.From<T>().Get();
            return response.Models;
        }

        public virtual async Task<List<T>> FilterByAsync(string columnName, object value, Supabase.Postgrest.Constants.Operator op = Supabase.Postgrest.Constants.Operator.Equals)
        {
            try
            {
                var response = await _supabaseClient.Client.From<T>().Filter(columnName, op, value).Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FilterBy error: {ex.Message}");
                return new List<T>();
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.Client.From<T>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id).Single();
            return response;

        }

        public virtual async Task<T?> AddAsync(T obj)
        {
            var response = await _supabaseClient.Client.From<T>().Insert(obj);
            return response.Models.FirstOrDefault();
        }

        public virtual async Task<T?> UpdateAsync(T obj)
        {
            var response = await _supabaseClient.Client.From<T>().Update(obj);
            return response.Models.FirstOrDefault();
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                await _supabaseClient.Client.From<T>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
                return true;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Delete failed: {ex.Message}");
                return false;
            }
        }
    }
}
