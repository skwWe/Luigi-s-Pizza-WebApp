using Supabase;

namespace PizzaWebApp.Services
{
    public class AuthService
    {
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private Supabase.Client _supabaseClient;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _supabaseUrl = _configuration["Supabase:Url"];
            _supabaseKey = _configuration["Supabase:Key"];
        }

        public async Task InitializeSupabase()
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true
            };

            _supabaseClient = new Supabase.Client(_supabaseUrl, _supabaseKey, options);
            await _supabaseClient.InitializeAsync();

        }
    }
}
