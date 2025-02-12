using System.ComponentModel.DataAnnotations.Schema;

namespace TemplatePustokApp.Models
{
    public class Order:BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string Country { get; set; }
        public string TownCity { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled,
    }
}
