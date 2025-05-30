using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Linq.Expressions;

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
            try
            {
                var session = await _client.Auth.SignUp(email, password);
                return session;
            }
            catch (Exception ex)
            {
                var session = await _client.Auth.SignUp(email, password);
                return session;
            }
        }
    }
}
