using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Core.Interfaces
{
    public interface IBaseService<T> where T : BaseModel, new()
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> FilterByAsync(string columnName, object value, Supabase.Postgrest.Constants.Operator op = Supabase.Postgrest.Constants.Operator.Equals);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> AddAsync(T obj);
        Task<T?> UpdateAsync(T obj);
        Task<bool> DeleteAsync(Guid id);
    }
}
