using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PizzaWebApp.Models
{
    [Table("users")] // Или "profiles", если таблица называется иначе
    public class UserProfile : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("email")]
        public string Email { get; set; }
    }
}
