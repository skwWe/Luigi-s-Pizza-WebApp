using SupabaseSession = Supabase.Gotrue.Session;

namespace AuthServiceTool
{
    public class SupabaseAuthWrapper : ISupabaseAuthWrapper
    {
        private readonly Supabase.Client _client;

        public SupabaseAuthWrapper(Supabase.Client client)
        {
            _client = client;
        }

        public async Task<SupabaseSession?> SignUp(string email, string password)
        {
            try
            {
                return await _client.Auth.SignUp(email, password);
            }
            catch
            {
                // Retry once
                return await _client.Auth.SignUp(email, password);
            }
        }
    }
}