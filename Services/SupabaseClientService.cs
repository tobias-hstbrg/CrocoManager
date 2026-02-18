using CrocoManager.Interfaces;
using Microsoft.Extensions.Configuration;
using Supabase;
using System;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class SupabaseClientService : ISupabaseClientService
    {
        private readonly Client _client;

        public SupabaseClientService(IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"];
            var key = configuration["Supabase:AnonKey"];

            if(string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
                throw new InvalidOperationException("Supabase URL or Anon Key is not configured properly.");

            _client = new Client(url, key);
        }

        public async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }

        public Client Client => _client;
    }
}
