using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaWebApp.Models;
using Supabase;
using PizzaWebApp.Models;

namespace PizzaWebApp.Services
{
    public class OrderService
    {
        private readonly Supabase.Client _client;

        public OrderService(Supabase.Client client) => _client = client;

        public async Task<Order> CreateOrder(List<CartItem> items)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderTime = DateTime.Now,
                Status = "created"
            };

            var orderResponse = await _client.From<Order>().Insert(order);
            var createdOrder = orderResponse.Models.First();

            var orderItems = items.Select(item => new OrderItem
            {
                ItemId = Guid.NewGuid(),
                OrderId = createdOrder.OrderId,
                MenuId = item.Item.Id,
                Quantity = item.Quantity,
                Comment = item.Comment
            }).ToList();

            await _client.From<OrderItem>().Insert(orderItems);

            return createdOrder;
        }
    }
}
