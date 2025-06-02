using Newtonsoft.Json;
using Supabase.Postgrest.Models;
using System;
using Supabase.Postgrest.Attributes;

namespace PizzaWebApp.Models
{
    using Supabase.Postgrest.Attributes;

    [Table("orders")]
    public class Order : BaseModel
    {
        [PrimaryKey("order_id")]
        [JsonProperty("order_id")]
        public Guid OrderId { get; set; }

        [Column("customer_name")]
        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [Column("customer_phone")]
        [JsonProperty("customer_phone")]
        public string? CustomerPhone { get; set; }

        [Column("order_time")]
        [JsonProperty("order_time")]
        public DateTime? OrderTime { get; set; }

        [Column("delivery_address")]
        [JsonProperty("delivery_address")]
        public string? DeliveryAddress { get; set; }

        [Column("status")]
        [JsonProperty("status")]
        public string? Status { get; set; }

        [Column("created_at")]
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}