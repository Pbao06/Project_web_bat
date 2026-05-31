using Getdata1.DTOs;
using Getdata1.Models;

namespace Getdata1.ViewModels
{
    public class StoreVM
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<int> LikedProductIds { get; set; } = new List<int>(); // nhờ có new list<int>() nên nếu null không sợ bị crash

        // Filters
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public string? ViewMode { get; set; }

        // Pagination
        public PaginationVM Pagination { get; set; } = new PaginationVM();

    }
}
