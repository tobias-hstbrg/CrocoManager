using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.Core.Interfaces;

namespace CrocoManager.Core.Services
{
    public abstract class BaseService<T> : IBaseService<T> where T : BaseModel, new()
    {
        protected readonly ISupabaseClientService _supabaseClient;

        public BaseService(ISupabaseClientService supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        protected void HandleException(Exception ex)
        {
            if (IsNetworkError(ex))
            {
                throw new Exception("Keine Internetverbindung. Bitte prüfen Sie Ihre Netzwerkverbindung.", ex);
            }
            throw ex;
        }

        private bool IsNetworkError(Exception? ex)
        {
            if (ex == null) return false;

            if (ex is HttpRequestException || 
                ex is System.Net.Sockets.SocketException ||
                ex.Message.Contains("Host is not reachable") || 
                ex.Message.Contains("Der gegebene Host ist nicht erreichbar") ||
                ex.Message.Contains("Failed to connect") ||
                ex.Message.Contains("Name or service not known"))
            {
                return true;
            }

            if (ex is AggregateException agg)
            {
                return agg.InnerExceptions.Any(IsNetworkError);
            }

            return IsNetworkError(ex.InnerException);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            try
            {
                var response = await _supabaseClient.Client.From<T>().Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<T>(); // Should not be reached if HandleException throws
            }
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
                HandleException(ex);
                return new List<T>();
            }
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabaseClient.Client.From<T>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id).Single();
                return response;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        public virtual async Task<T?> AddAsync(T obj)
        {
            try
            {
                var response = await _supabaseClient.Client.From<T>().Insert(obj);
                return response.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        public virtual async Task<T?> UpdateAsync(T obj)
        {
            try
            {
                var response = await _supabaseClient.Client.From<T>().Update(obj);
                return response.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                await _supabaseClient.Client.From<T>().Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString()).Delete();
                return true;
            }
            catch (Exception ex) 
            {
                HandleException(ex);
                return false;
            }
        }
    }
}
