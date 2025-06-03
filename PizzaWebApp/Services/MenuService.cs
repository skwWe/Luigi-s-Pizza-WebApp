using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaWebApp.Models;
using Supabase;

namespace PizzaWebApp.Services
{
    public class MenuService
    {
        private readonly Supabase.Client _supabase;
        private const string MenuBucketName = "menu-images";

        public MenuService(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<MenuItem>> GetBurgersAsync()
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
                Console.WriteLine($"Error loading burgers: {ex.Message}");
                return new List<MenuItem>();
            }
        }

        public async Task<string> GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "/images/pizza-placeholder.png";

            // Если URL уже полный (например, из корзины)
            if (imagePath.StartsWith("http"))
                return imagePath;

            try
            {
                // Убедитесь, что imagePath не начинается с /
                if (imagePath.StartsWith("/"))
                    imagePath = imagePath.Substring(1);

                // Используйте правильное имя бакета (должно совпадать в MenuService и CartService)
                return await _supabase.Storage
                    .From("avatars") // Замените на ваше имя бакета
                    .CreateSignedUrl(imagePath, 3600); // URL действителен 1 час
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating image URL: {ex.Message}");
                return "/images/pizza-placeholder.png";
            }
        }
    }
    }
