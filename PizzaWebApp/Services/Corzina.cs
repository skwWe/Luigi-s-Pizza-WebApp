using PizzaWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaWebApp.Services
{
    public class CartService
    {
        private readonly ISupabaseCartWrapper _wrapper;
        public List<CartItem> Items { get; } = new();
        public event Action OnChange;
        public Guid LastOrderId { get; set; }

        public CartService(ISupabaseCartWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        public async Task AddItemWithImage(MenuItem item)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                var itemCopy = new MenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = await _wrapper.GetImageUrl(item.ImageUrl)
                };

                Items.Add(new CartItem { Item = itemCopy, Quantity = 1 });
            }
            NotifyStateChanged();
        }

        public void AddItem(MenuItem item)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                Items.Add(new CartItem { Item = item, Quantity = 1 });
            }
            NotifyStateChanged();
        }

        public void AddOrUpdateItem(MenuItem item, int quantityChange = 1)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);

            if (existing != null)
            {
                existing.Quantity += quantityChange;

                if (existing.Quantity <= 0)
                {
                    Items.Remove(existing);
                }
            }
            else if (quantityChange > 0)
            {
                Items.Add(new CartItem { Item = item, Quantity = quantityChange });
            }

            NotifyStateChanged();
        }

        public void RemoveItem(MenuItem item)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);
            if (existing == null) return;

            if (existing.Quantity > 1)
                existing.Quantity--;
            else
                Items.Remove(existing);

            NotifyStateChanged();
        }

        public async Task<(bool success, Guid orderId, string shortId)> Checkout(string customerName, string phone, string address)
        {
            try
            {
                var moscowTime = DateTime.UtcNow.AddHours(3);
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerName = customerName,
                    CustomerPhone = phone,
                    DeliveryAddress = address,
                    OrderTime = moscowTime,
                    Status = "В обработке",
                    CreatedAt = moscowTime
                };

                var orderResponse = await _wrapper.CreateOrder(order);
                var insertedOrder = orderResponse.Models.FirstOrDefault();

                if (insertedOrder == null)
                    return (false, Guid.Empty, string.Empty);

                var orderItems = Items.Select(item => new OrderItem
                {
                    ItemId = Guid.NewGuid(),
                    OrderId = insertedOrder.OrderId,
                    MenuId = item.Item.Id,
                    Quantity = item.Quantity,
                    Comment = item.Comment
                }).ToList();

                await _wrapper.CreateOrderItems(orderItems);

                var orderView = await _wrapper.GetOrderDetails(insertedOrder.OrderId);

                Clear();
                LastOrderId = insertedOrder.OrderId;

                return (true, insertedOrder.OrderId, orderView?.ShortId ?? string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при оформлении заказа: {ex.Message}");
                return (false, Guid.Empty, string.Empty);
            }
        }

        public void Clear()
        {
            Items.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}