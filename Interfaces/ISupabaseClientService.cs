using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Interfaces
{
    public interface ISupabaseClientService
    {
        Client Client { get; }
        Task InitializeAsync();
    }
}
