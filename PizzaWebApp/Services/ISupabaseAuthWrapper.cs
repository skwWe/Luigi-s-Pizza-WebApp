using SupabaseSession = Supabase.Gotrue.Session;

namespace AuthServiceTool
{
    public interface ISupabaseAuthWrapper
    {
        Task<SupabaseSession?> SignUp(string email, string password);
    }
}