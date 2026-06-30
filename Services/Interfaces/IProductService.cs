using Getdata1.DTOs;

namespace Getdata1.Services.Interfaces
{
    // dùng để giới thiệu cho controller biết là có gì , và Tính Đa hình 
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<List<ProductDto>> GetWistlistByUserIdAsync(int userId);
        Task<bool> ClearAllFavoriteAsync(int userId);
        Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetFilteredProductsAsync(
            int? categoryId, string? searchTerm, string? brand, decimal? minPrice, decimal? maxPrice, string? sortBy, int page, int pageSize);
        Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int categoryId, int currentProductId, int count = 4);
        Task<IEnumerable<ProductDto>> SearchQuickAsync(string query, int limit = 5);
        //Task<Top1ProductDTO> GetTop1Product();
        Task<int> Laylowstock();
    }
}