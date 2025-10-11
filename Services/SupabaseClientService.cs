using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public sealed class SupabaseClientService
    {
        private static readonly Lazy<SupabaseClientService> _instance = new(() => new SupabaseClientService());

        private Supabase.Client? _client;
        private bool _isInitialized = false;

        public static SupabaseClientService Instance => _instance.Value;

        private SupabaseClientService() { }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            // Initalizing Supabase connection
            _client = new Client(ConfigLoader.Configuration["Supabase:Url"], ConfigLoader.Configuration["Supabase:AnonKey"]);
            await _client.InitializeAsync();

            _isInitialized = true;
        }

        public Client Client
        {
            get
                            {
                if (!_isInitialized || _client == null)
                {
                    throw new InvalidOperationException("SupabaseClientService is not initialized. Call InitializeAsync() first.");
                }
                return _client;
            }
        }
    }
}
