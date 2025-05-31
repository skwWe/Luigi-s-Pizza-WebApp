using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

public interface ISupabaseClient
{
    IGotrueClient<User, Session> Auth { get; }
}
