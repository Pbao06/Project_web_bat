using Getdata1.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Getdata1.Models
{
    public class _Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [NotMapped]  // ← add this
        public decimal TotalPrice => OrderItems.Sum(i => i.Price * i.Quantity);

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Shipping Information
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public string? Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string District { get; set; }
        public string? ZipCode { get; set; }
        public string? OrderNotes { get; set; }

        // Payment & Shipping Methods
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? PromoCode { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
