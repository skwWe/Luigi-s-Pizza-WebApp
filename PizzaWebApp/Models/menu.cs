using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PizzaWebApp.Models
{
    [Table("menu")]
    public class MenuItem : BaseModel
    {
        [PrimaryKey("menu_id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("base_price")]
        public decimal Price { get; set; }

        [Column("image_url")] // Поле где хранится имя файла или URL
        public string ImageUrl { get; set; }
    }
}
