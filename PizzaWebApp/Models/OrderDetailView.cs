using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PizzaWebApp.Models;

[Table("order_details_view")]
public class OrderDetailView : BaseModel
{
    public OrderDetailView() { }

    [PrimaryKey("order_id", false)]
    public Guid OrderId { get; set; }

    [Column("short_id")]
    public string ShortId { get; set; }

    [Column("order_time")]
    public DateTime OrderTime { get; set; }

    [Column("status")]
    public string Status { get; set; }

    [Column("pizza_name")]
    public string PizzaName { get; set; }

    [Column("ingredients")]
    public List<string> Ingredients { get; set; }

    [Column("toppings")]
    public List<string> Toppings { get; set; }

    [Column("comment")]
    public string Comment { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }
}
