using PizzaWebApp.Models;
using Supabase;
using Supabase.Postgrest.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaWebApp.Services
{
    public class SupabaseCartWrapper : ISupabaseCartWrapper
    {
        private readonly Supabase.Client _supabase;

        public SupabaseCartWrapper(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<string> GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            try
            {
                return await _supabase.Storage
                    .From("avatars")
                    .CreateSignedUrl(imagePath, 3600);
            }
            catch
            {
                return "/images/pizza-placeholder.png";
            }
        }

        public async Task<ModeledResponse<Order>> CreateOrder(Order order)
        {
            return await _supabase.From<Order>().Insert(order);
        }

        public async Task<ModeledResponse<OrderItem>> CreateOrderItems(List<OrderItem> items)
        {
            return await _supabase.From<OrderItem>().Insert(items);
        }

        public async Task<OrderDetailView> GetOrderDetails(Guid orderId)
        {
            return await _supabase.From<OrderDetailView>()
                .Where(x => x.OrderId == orderId)
                .Single();
        }
    }
}