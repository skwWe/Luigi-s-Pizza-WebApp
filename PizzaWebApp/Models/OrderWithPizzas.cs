
namespace PizzaWebApp.Models
{
    public class OrderWithPizzas
    {
        public Guid OrderId { get; set; }
        public string ShortId { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }

        public List<PizzaInOrder> Pizzas { get; set; } = new List<PizzaInOrder>();
    }


    public class PizzaInOrder
    {
        public string PizzaName { get; set; }
        public List<string> Ingredients { get; set; } = new List<string>();
        public List<string> Toppings { get; set; } = new List<string>();
        public string Comment { get; set; }
        public int Quantity { get; set; }
    }
}
