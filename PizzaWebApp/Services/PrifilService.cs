// Services/ProfileService.cs
using Microsoft.JSInterop;
using PizzaWebApp.Models;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace YourNamespace.Services
{
    public class ProfileService
    {
        private readonly Supabase.Client _supabase;

        public ProfileService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        // Модель профиля


        // Метод получения профиля
        public async Task<(string FirstName, string LastName, string Email)?> GetProfileAsync()
        {
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return null;

            try
            {
                var response = await _supabase
                    .From<profiles>()
                    .Select("first_name, last_name, email")
                    .Where(x => x.UserId == user.Id)
                    .Single();

                return (response.FirstName, response.LastName, response.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile loading error: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateProfileAsync(string firstName, string lastName, string Email)
        {
            try
            {
                var user = _supabase.Auth.CurrentUser;
                if (user == null)
                {
                    Console.WriteLine("User not authenticated");
                    return;
                }

                var result = await _supabase
                    .From<profiles>()
                    .Where(x => x.UserId == user.Id)
                    .Set(x => x.FirstName, firstName)
                    .Set(x => x.LastName, lastName)
                    .Set(x => x.Email, Email)
                    .Update();

                if (result.ResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Profile updated successfully");
                }
                else
                {
                    Console.WriteLine($"Update failed: {result.ResponseMessage.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex.Message}");
                throw; // Можно обработать ошибку в компоненте
            }
        }


       


    }
   

}
