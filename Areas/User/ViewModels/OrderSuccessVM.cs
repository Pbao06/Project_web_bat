using Getdata1.DTOs;
using Getdata1.Models.Enums;

namespace Getdata1.Areas.User.ViewModels
{
    public class OrderSuccessVM
    {
        public int OrderId { get; set; }
        public string OrderNumber => $"#PB-{OrderDate:yyyyMMdd}-{OrderId:D4}";
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        
        // Items
        public List<OrderItemDto> Items { get; set; } = new();
        
        // Summary
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? PromoCode { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }
        
        // Shipping Info
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        
        // Estimation
        public DateTime EstimatedDeliveryMin => OrderDate.AddDays(3);
        public DateTime EstimatedDeliveryMax => OrderDate.AddDays(5);
    }
}
