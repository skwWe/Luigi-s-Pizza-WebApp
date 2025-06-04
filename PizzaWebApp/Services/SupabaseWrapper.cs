namespace PizzaWebApp.Services
{
    using Supabase;
    using PizzaWebApp.Models;

    public class SupabaseWrapper : ISupabaseWrapper
    {
        private readonly Client _client;

        public SupabaseWrapper(Client client)
        {
            _client = client;
        }

        public async Task<List<OrderDetailView>> GetOrdersWithStatusAsync(string status)
        {
            var response = await _client
                .From<OrderDetailView>()
                .Where(x => x.Status == status)
                .Get();

            return response.Models;
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            await _client
                .From<Order>()
                .Where(x => x.OrderId == orderId)
                .Set(x => x.Status, newStatus)
                .Update();
        }
    }

}
