using CrocoManager.DTOs;
using CrocoManager.Interfaces;
using CrocoManager.Models;
using Supabase;
using Supabase.Postgrest.Exceptions;
using Supabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Supabase.Functions.Client;

namespace CrocoManager.Services
{
    public sealed class SupabaseAuthService : IAuthService
    {
        private readonly SupabaseClientService _supabase;

        public SupabaseAuthService(SupabaseClientService supabase)
        {
            _supabase = supabase;
        }
        public async Task InitializeAsync()
        {
            await _supabase.InitializeAsync();
        }


        public async Task<SupabaseSession?> RegisterAsync(string email, string password)
        {
            try
            {
                var whitelistResponse = await CheckEmailWhitelist(email);
                if (whitelistResponse == null)
                    return null;

                // grab Userrole in usable format
                var newUsersRole = ParseUserRole(whitelistResponse.role);

                // Prepare user_metadata for new user
                var options = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "role", newUsersRole.ToString() }
                    }
                };

                // retrieves a supabase session and the new user (hopefully)
                var authResponse = await _supabase.Client.Auth.SignUp(email, password, options);
                if (authResponse?.User == null)
                    return null;

                var session = BuildSession(authResponse);

                // safe session in secure storage of the operting system
                var sessionJson = JsonSerializer.Serialize(session);
                await SecureStorage.SetAsync("supabase_session", sessionJson);

                return session;
            }
            catch (PostgrestException ex)
            {
                Console.WriteLine($"Supabase error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error {ex.Message}");
            }
            return null;
        }

        private async Task<EmailWhitelist?> CheckEmailWhitelist(string email)
        {
            try
            {
                var response = await _supabase.Client
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

        private SupabaseSession BuildSession(Supabase.Gotrue.Session authResponse)
        {
            var userMetadata = authResponse.User.UserMetadata;
            var role = ParseUserRole(userMetadata?["role"]?.ToString());

            return new SupabaseSession
            {
                AccessToken = authResponse.AccessToken,
                RefreshToken = authResponse.RefreshToken,
                TokenType = authResponse.TokenType,
                ExpiresIn = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn),
                User = new Models.User
                {
                    Id = authResponse.User.Id,
                    Email = authResponse.User.Email ?? string.Empty,
                    CreatedAt = authResponse.User.CreatedAt,
                    UserMetadata = new Models.UserMetadata
                    {
                        Role = role
                    }
                }
            };
        }

        private UserRole ParseUserRole(string? role)
        {
            if (string.IsNullOrEmpty(role))
                return UserRole.NotAssigned;

            // Tries to parse string into enum if that doesnt work it defaults to not Assigned just like if the string would be null.
            return Enum.TryParse(role, true, out UserRole parsedRole) ? parsedRole : UserRole.NotAssigned;
        }


        public async Task<SupabaseSession?> LoginAsync(string email, string password)
        {
            SupabaseSession session = new SupabaseSession();
            try
            {
                var authResponse = await _supabase.Client.Auth.SignInWithPassword(email, password);
                
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
                var result = await _supabase.Client.Auth.RetrieveSessionAsync();
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
