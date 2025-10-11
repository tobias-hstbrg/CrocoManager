using CrocoManager.Interfaces;
using CrocoManager.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public sealed class SupabaseAuthService : IAuthService
    {
        private readonly Client _client;

        public SupabaseAuthService(Client client)
        {
            _client = client;
        }
        //public static async Task<SupabaseAuthService> CreateAsync()
        //{
        //    await SupabaseClientService.Instance.InitializeAsync();
        //    return new SupabaseAuthService(SupabaseClientService.Instance.Client);
        //}

        // Creates Supabase Client Instance
        public static async Task<SupabaseAuthService> CreateAsync()
        {
            await SupabaseClientService.Instance.InitializeAsync();
            return new SupabaseAuthService(SupabaseClientService.Instance.Client);
        }


        public async Task<Supabase.Gotrue.User?> RegisterAsync(string email, string password)
        {
            try
            {
                // retrieves a supabase session and user (hopefully)
                var result = await _client.Auth.SignUp(email, password);

                if(result != null)
                {
                    return result.User;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var result = await _client.Auth.RetrieveSessionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetTextMessageAsync()
        {
            try
            {
                var result = await _client.From<Test>().Get();
                if (result.Models.Count > 0)
                {
                    return result.Models[0].text;
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
            return null;
        }
    }
}
