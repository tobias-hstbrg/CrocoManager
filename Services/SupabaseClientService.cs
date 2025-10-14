using Supabase;
using System;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class SupabaseClientService
    {
        private readonly Client _client;

        public SupabaseClientService()
        {
            var url = ConfigLoader.Configuration["Supabase:Url"];
            var key = ConfigLoader.Configuration["Supabase:AnonKey"];

            _client = new Client(url, key);
        }

        public async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }

        public Client Client => _client;
    }
}
