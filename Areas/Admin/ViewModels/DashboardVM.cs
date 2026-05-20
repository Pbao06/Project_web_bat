using Getdata1.Models;

namespace Getdata1.Areas.Admin.ViewModels
{
    public class DashboardVM
    {
        // get what we need for UI 
        public int TotalOrders { get; set; }
        public int TotalCustomer { get; set; }
        public decimal TotalSales { get; set; }
        public int PendingOrders { get; set; }

        public List<_Order>? RecentOrders { get; set; }
        public List<Product>? LowStockProducts { get; set; }
        public List<Product>? MissingImageProducts { get; set; }
    }
}
