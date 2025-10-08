using Supabase;
using CrocoManager.Models;
using dotenv.net;

namespace CrocoManager.Services;

public sealed class SupabaseService
{
    private static readonly Lazy<SupabaseService> lazy = new(() => new SupabaseService());
    public static SupabaseService Instance => lazy.Value;

    private Supabase.Client _client;
    public bool IsInitialized => _client != null;

    private SupabaseService() { }

    public async Task InitializeAsync()
    {
        DotEnv.Load();

        string url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? throw new Exception("SUPABASE_URL missing");
        string key = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? throw new Exception("SUPABASE_ANON_KEY missing");

        if (IsInitialized) return;

        _client = new Client( url, key );
        await _client.InitializeAsync();
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (!IsInitialized) await InitializeAsync();

        try
        {
            var session = await _client.Auth.SignIn(email, password);
            if (session?.User != null)
            {
                return new User
                {
                    Id = session.User.Id,
                    Email = session.User.Email ?? "",
                    Username = session.User.Email ?? "" // no username in Supabase auth, fallback to email
                };
            }
        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            if (!IsInitialized) await InitializeAsync();
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
        if (!IsInitialized) await InitializeAsync();

        try
        {
            var result = await _client.From<Test>().Get();
            if(result.Models.Count > 0)
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
