using Getdata1.Models.Enums;

namespace Getdata1.Areas.Admin.ViewModels
{
    public class OrderIndexVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string? UserName { get; set; }  // added
    }
    public class OrderVM // this detail VM
    {
        // things i need to show in UI 
        public int OrderId { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set;  }
       
        public List<OrderItemVM> Items { get; set; } = new List<OrderItemVM>();
        //cause totalprice sum all orderitem 
        public decimal TotalPrice => Items.Sum(i => i.Subtotal);


    }
    public class OrderItemVM
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
    public class OrderPageVM
    {
        public List<OrderIndexVM> Orders { get; set; } = new List<OrderIndexVM>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Status { get; set; }   // ← for filter
        public string? Name { get; set; }     // ← keep pagination compatible
        public int? CategoryId { get; set; }  // ← keep pagination compatible
    }
}
