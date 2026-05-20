using Getdata1.Models;

namespace Getdata1.Areas.Admin.ViewModels
{
    public class ProductFilterVM
    {
        // filter inputs — all nullable because user might not fill them
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        // filter 
        // result list — holds the filtered products
        // 👇 PART 2 — product list to display

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        // method to pagination 
        // The pagination "needs" these:
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
