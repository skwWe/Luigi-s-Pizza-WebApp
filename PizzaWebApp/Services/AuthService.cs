using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AuthServiceTool
{
    public class AuthService
    {
        private readonly Supabase.Client _client;

        public AuthService(Supabase.Client client)
        {
            _client = client;
        }

        public async Task<Session?> RegisterAsync(string email, string password)
        {
            var session = await _client.Auth.SignUp(email, password);
            return session;
        }

    }
}

