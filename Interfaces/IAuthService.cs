using CrocoManager.Models;
using Supabase;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Interfaces
{
    public interface IAuthService
    {
        Task<bool> TestConnectionAsync();
        Task<string?> GetTextMessageAsync();
        Task<SupabaseSession?> RegisterAsync(string email, string password);
        Task<SupabaseSession?> LoginAsync(string email, string password);
    }
}
