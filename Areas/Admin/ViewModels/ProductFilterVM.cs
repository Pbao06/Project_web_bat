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
        public string? Status { get; set; }
        // result list — holds the filtered products
        // 👇 PART 2 — product list to display
        // filter for Brand
        public string? Search_Brand { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        // method to pagination 
        // The pagination "needs" these:
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
