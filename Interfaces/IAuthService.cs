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
        Task<User?> RegisterAsync(string email, string password);
    }
}
