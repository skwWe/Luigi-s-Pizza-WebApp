using SupabaseSession = Supabase.Gotrue.Session;

namespace AuthServiceTool
{
    public class AuthService
    {
        private readonly ISupabaseAuthWrapper _authWrapper;

        public AuthService(Supabase.Client client) : this(new SupabaseAuthWrapper(client))
        {
        }

        public AuthService(ISupabaseAuthWrapper authWrapper)
        {
            _authWrapper = authWrapper;
        }

        public async Task<SupabaseSession?> RegisterAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));
            try
            {
                return await _authWrapper.SignUp(email, password); // Регистрация в Supabase
            }
            catch
            {
                // Повторная попытка при ошибке
                return await _authWrapper.SignUp(email, password);
            }
        }
    }
}