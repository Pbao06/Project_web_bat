using Getdata1.Models;

namespace Getdata1.Areas.Admin.ViewModels
{
    public class CustomerVM
    {
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }

    }
    public class CustomerIndexVM
    {
        public List<CustomerVM> Customers { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        // add more somethings 

    }

}
