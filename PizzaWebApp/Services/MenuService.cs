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
        private readonly ISupabaseMenuWrapper _wrapper;
        public MenuService(ISupabaseMenuWrapper wrapper)
        {
            _wrapper = wrapper;
        }



        public async Task<List<MenuItem>> GetBurgersAsync()
        {
            try
            {
                return await _wrapper.GetMenuItems();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading burgers: {ex.Message}");
                return new List<MenuItem>(); // Возвращаем пустой список при ошибке
            }
        }

        public async Task<string> GetImageUrl(string imagePath)
        {
            return await _wrapper.GetImageUrl(imagePath);
        }
    }
    }
