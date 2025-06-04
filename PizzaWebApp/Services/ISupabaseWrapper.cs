namespace PizzaWebApp.Services
{
    using PizzaWebApp.Models;

    public interface ISupabaseWrapper
    {
        Task<List<OrderDetailView>> GetOrdersWithStatusAsync(string status);
        Task UpdateOrderStatusAsync(Guid orderId, string newStatus);
    }

}
