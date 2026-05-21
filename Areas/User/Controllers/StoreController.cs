using Getdata1.Services.Interfaces;
using Getdata1.ViewModels;
using Getdata1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    public class StoreController : Controller
    {
        private readonly IProductService _productService;

        public StoreController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(
            int? categoryId, 
            string? searchTerm, 
            decimal? minPrice, 
            decimal? maxPrice, 
            string? sortBy, 
            int page = 1)
        {
            try
            {
                const int pageSize = 12;

                // Sử dụng service đã nâng cấp để lấy dữ liệu có lọc và phân trang
                var (products, totalCount) = await _productService.GetFilteredProductsAsync(
                    categoryId, searchTerm, minPrice, maxPrice, sortBy, page, pageSize);

                var categories = await _productService.GetAllCategoriesAsync();

                var viewModel = new StoreVM
                {
                    Products = products,
                    Categories = categories,
                    CategoryId = categoryId,
                    SearchTerm = searchTerm,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SortBy = sortBy,
                    Pagination = new PaginationVM
                    {
                        CurrentPage = page,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                        ActionName = "Index",
                        ControllerName = "Store",
                        AreaName = "User",
                        RouteValues = new Dictionary<string, string>
                        {
                            { "categoryId", categoryId?.ToString() ?? "" },
                            { "searchTerm", searchTerm ?? "" },
                            { "minPrice", minPrice?.ToString() ?? "" },
                            { "maxPrice", maxPrice?.ToString() ?? "" },
                            { "sortBy", sortBy ?? "" }
                        }
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Bắt lỗi và trả về mã lỗi 500 theo quy định
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var relatedProducts = await _productService.GetRelatedProductsAsync(product.CategoryId, product.Id, 8);
                ViewBag.RelatedProducts = relatedProducts;

                return View(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
