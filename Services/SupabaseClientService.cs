using dotenv.net;
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
            DotEnv.Load();

            string url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? throw new Exception("SUPABASE_URL missing");
            string key = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? throw new Exception("SUPABASE_ANON_KEY missing");

            if (_isInitialized) return;

            _client = new Client(url, key);
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
