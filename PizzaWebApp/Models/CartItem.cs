namespace PizzaWebApp.Models
{
    public class CartItem
    {
        public MenuItem Item { get; set; } // Хранит все данные о товаре, включая изображение
        public int Quantity { get; set; }
        public string? Comment { get; set; }
        public decimal TotalPrice => Item.Price * Quantity;
    }
}
