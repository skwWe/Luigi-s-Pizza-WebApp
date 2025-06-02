using PizzaWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;

namespace PizzaWebApp.Services
{
    public class CartService
    {
        private readonly Client _supabase;
        public List<CartItem> Items { get; } = new();
        public event Action OnChange;
        public CartService(Client supabase)
        {
            _supabase = supabase;
        }
        public Guid LastOrderId { get; set; } // Сохраняем ID последнего заказа
        public async Task AddItemWithImage(MenuItem item)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                // Создаем копию с обновленным URL изображения
                var itemCopy = new MenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = await GetImageUrl(item.ImageUrl)
                };

                Items.Add(new CartItem { Item = itemCopy, Quantity = 1 });
            }
            NotifyStateChanged();
        }

        public async Task AddItem(MenuItem item)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                // Создаем полную копию элемента с изображением
                var itemCopy = new MenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl // Сохраняем оригинальный URL
                };

                Items.Add(new CartItem { Item = itemCopy, Quantity = 1 });
            }
            NotifyStateChanged();
        }
        private async Task<string> GetImageUrl(string imagePath)
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
        public void AddOrUpdateItem(MenuItem item, int quantityChange = 1)
        {
            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);

            if (existing != null)
            {
                existing.Quantity += quantityChange;

                // Удаляем если количество стало 0 или меньше
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
                // 1. Берем текущее локальное время (как на компьютере)
                var moscowTime = DateTime.UtcNow.AddHours(3); 

               // 2. Создаем заказ
               var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerName = customerName,
                    CustomerPhone = phone,
                    DeliveryAddress = address,
                   OrderTime = moscowTime, // Сохраняем как есть
                   Status = "В обработке",
                   CreatedAt = moscowTime
               };

                // 3. Логируем для проверки
              

                // 4. Отправляем в базу
                var orderResponse = await _supabase.From<Order>().Insert(order);
                var insertedOrder = orderResponse.Models.FirstOrDefault();

                if (insertedOrder == null)
                    return (false, Guid.Empty, string.Empty);

                // 5. Сохраняем позиции заказа
                var orderItems = Items.Select(item => new OrderItem
                {
                    ItemId = Guid.NewGuid(),
                    OrderId = insertedOrder.OrderId,
                    MenuId = item.Item.Id,
                    Quantity = item.Quantity,
                    Comment = item.Comment
                }).ToList();

                await _supabase.From<OrderItem>().Insert(orderItems);

                // 6. Получаем ShortId заказа
                var orderView = await _supabase.From<OrderDetailView>()
                    .Where(x => x.OrderId == insertedOrder.OrderId)
                    .Single();

                // 7. Очищаем корзину
                Clear();

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