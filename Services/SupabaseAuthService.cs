using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using Supabase;
using Supabase.Postgrest.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Supabase.Functions.Client;

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


        public async Task<SupabaseSession?> RegisterAsync(string email, string password)
        {
            try
            {
                var whitelistResponse = await CheckEmailWhitelist(email);

                if (whitelistResponse == null)
                    return null;

                // Setting the role for the new user
                var newUsersRole = new UserRole();
                if (string.IsNullOrEmpty(whitelistResponse.role))
                    newUsersRole = UserRole.NotAssigned;

                if (!Enum.TryParse<UserRole>(whitelistResponse.role, out newUsersRole))
                    newUsersRole = UserRole.NotAssigned;

                var options = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                {
                    { "role", newUsersRole.ToString() } // store as string, e.g. "Admin"
                }
                };

                // retrieves a supabase session and the new user (hopefully)
                var authResponse = await _client.Auth.SignUp(email, password, options);
                SupabaseSession session = new SupabaseSession();

                if(authResponse != null && authResponse.User != null)
                {
                    session.AccessToken = authResponse.AccessToken;
                    session.RefreshToken = authResponse.RefreshToken;
                    session.TokenType = authResponse.TokenType;
                    session.ExpiresIn = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn);

                    session.User = new Models.User
                    {
                        Id = authResponse.User.Id,
                        Email = authResponse.User.Email ?? string.Empty,
                        CreatedAt = authResponse.User.CreatedAt,
                        UserMetadata = new Models.UserMetadata
                        {
                            Role = Enum.TryParse(authResponse.User.UserMetadata?["role"]?.ToString(), true, out UserRole parsedRole)
                                                 ? parsedRole
                                                 : UserRole.NotAssigned
                        }
                    };
                    var sessionJson = JsonSerializer.Serialize(session);
                    await SecureStorage.SetAsync("supabase_session", sessionJson);
                    return session;
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

        private async Task<EmailWhitelist?> CheckEmailWhitelist(string email)
        {
            try
            {
                var response = await _client
                    .From<EmailWhitelist>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Single();

                return response;
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine($"PostgrestException: {ex.Message}");
                if (ex.Message.Contains("No rows"))
                    return null;

                throw; // rethrow other Postgrest errors
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex}");
                throw; // rethrow to see where it bubbles up
            }
        }


        public async Task<SupabaseSession?> LoginAsync(string email, string password)
        {
            SupabaseSession session = new SupabaseSession();
            try
            {
                var authResponse = await _client.Auth.SignInWithPassword(email, password);
                
                if (authResponse != null && authResponse.User != null)
                {
                    session.AccessToken = authResponse.AccessToken;
                    session.RefreshToken = authResponse.RefreshToken;
                    session.TokenType = authResponse.TokenType;
                    session.ExpiresIn = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn);

                    session.User = new Models.User
                    {
                        Id = authResponse.User.Id,
                        Email = authResponse.User.Email ?? string.Empty,
                        CreatedAt = authResponse.User.CreatedAt,
                        UserMetadata = new Models.UserMetadata
                        {
                            Role = Enum.TryParse(authResponse.User.UserMetadata?["role"]?.ToString(), true, out UserRole role)
                                                 ? role
                                                 : UserRole.NotAssigned
                        }
                    };

                    var sessionJson = JsonSerializer.Serialize(session);
                    await SecureStorage.SetAsync("supabase_session", sessionJson);
                }
                return session;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }
            return null;
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
            //try
            //{
            //    var result = await _client.From<Test>().Get();
            //    if (result.Models.Count > 0)
            //    {
            //        return result.Models[0].text;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return $"Error: {ex.Message}";
            //}
            return null;
        }
    }
}
