using PizzaWebApp.Models;
using Supabase.Postgrest.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaWebApp.Services
{
    public interface ISupabaseCartWrapper
    {
        Task<string> GetImageUrl(string imagePath);
        Task<ModeledResponse<Order>> CreateOrder(Order order);
        Task<ModeledResponse<OrderItem>> CreateOrderItems(List<OrderItem> items);
        Task<OrderDetailView> GetOrderDetails(Guid orderId);
    }
}