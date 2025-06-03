using PizzaWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaWebApp.Services
{
    public interface ISupabaseMenuWrapper
    {
        Task<List<MenuItem>> GetMenuItems();
        Task<string> GetImageUrl(string imagePath);
    }
}