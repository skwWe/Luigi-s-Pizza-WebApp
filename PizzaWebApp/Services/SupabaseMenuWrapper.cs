using PizzaWebApp.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaWebApp.Services
{
    public class SupabaseMenuWrapper : ISupabaseMenuWrapper
    {
        private readonly Client _supabase;

        public SupabaseMenuWrapper(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<MenuItem>> GetMenuItems()
        {
            try
            {
                var response = await _supabase
                    .From<MenuItem>()
                    .Select("*")
                    .Get();

                return response.Models;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading menu items: {ex.Message}");
                return new List<MenuItem>();
            }
        }

        public async Task<string> GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "/images/pizza-placeholder.png";

            if (imagePath.StartsWith("http"))
                return imagePath;

            try
            {
                if (imagePath.StartsWith("/"))
                    imagePath = imagePath.Substring(1);

                return await _supabase.Storage
                    .From("avatars")
                    .CreateSignedUrl(imagePath, 3600);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating image URL: {ex.Message}");
                return "/images/pizza-placeholder.png";
            }
        }
    }
}