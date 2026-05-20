namespace Getdata1.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal SubTotal => Items.Sum(i => i.Price * i.Quantity);
        public decimal DiscountAmount { get; set; }
        public string? AppliedPromoCode { get; set; }
        public decimal ShippingFee { get; set; } = 0; // Default to free or calculated
        public decimal TotalPrice => SubTotal - DiscountAmount + ShippingFee;
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? CategoryName { get; set; }
        public string? VariantInfo { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}