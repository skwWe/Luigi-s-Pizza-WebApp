using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PizzaWebApp.Models
{
    [Table("order_items")]
    public class OrderItem : BaseModel
    {
        [PrimaryKey("order_item_id")]
        public Guid ItemId { get; set; } = Guid.NewGuid();

        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Column("menu_id")]
        public string MenuId { get; set; }  // Переименовал ProductId в MenuId

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }
    }

}
