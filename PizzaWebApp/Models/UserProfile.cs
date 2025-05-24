using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PizzaWebApp.Models
{
    [Table("profiles")]
    public class UserProfile : BaseModel
    {
        [PrimaryKey("user_id", false)]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("age")]
        public int Age { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;
    }
}
