using Getdata1.Areas.User.ViewModels;
using Getdata1.Data;
using Getdata1.DTOs;
using Getdata1.Models;
using Getdata1.Services.Implementations;
using Getdata1.Services.Interfaces;
using Getdata1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    public class StoreController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        public StoreController(IProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult> Liked(string sortBy) // mở trang liked 
        {

            // 1. Lấy ID người dùng đang đăng nhập
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "User" });
            }
            // 2. Lấy danh sách sản phẩm YÊU THÍCH từ Database (thông qua Service)
            var likedProducts = await _productService.GetWistlistByUserIdAsync(userId);

            // 3. (Tùy chọn) Xử lý sắp xếp (Sort) nếu user chọn trên giao diện
            if (sortBy == "price_asc")
                likedProducts = likedProducts.OrderBy(p => p.Price).ToList();
            else if (sortBy == "price_desc")
                likedProducts = likedProducts.OrderByDescending(p => p.Price).ToList();

            // 4. Nhét danh sách đã lọc vào ViewModel
            var viewModel = new LikeVM
            {
                Products = likedProducts, // KHÚC NÀY QUAN TRỌNG: Chỉ truyền list yêu thích vào đây!
                SortBy = sortBy
            };

            // 5. Ném ra View để Razor chạy foreach
            return View(viewModel);
        }
        public async Task<IActionResult> Index(
            int? categoryId,
            string? searchTerm,
            string? brand,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            string? viewMode = "grid",
            int page = 1)
        {
            try
            {
                const int pageSize = 12;

                // Sử dụng service đã nâng cấp để lấy dữ liệu có lọc và phân trang
                var (products, totalCount) = await _productService.GetFilteredProductsAsync(
                    categoryId, searchTerm, brand, minPrice, maxPrice, sortBy, page, pageSize);

                var categories = await _productService.GetAllCategoriesAsync();

                var viewModel = new StoreVM
                {
                    Products = products,
                    Categories = categories,
                    CategoryId = categoryId,
                    SearchTerm = searchTerm,
                    Brand = brand,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SortBy = sortBy,
                    ViewMode = viewMode,
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
                            { "brand", brand ?? "" },
                            { "minPrice", minPrice?.ToString() ?? "" },
                            { "maxPrice", maxPrice?.ToString() ?? "" },
                            { "sortBy", sortBy ?? "" },
                            { "viewMode", viewMode ?? "grid" }
                        }
                    }
                };

                // Lấy danh sách sản phẩm đã thích nếu user đã đăng nhập
                if (User.Identity.IsAuthenticated)
                {
                    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (int.TryParse(userIdString, out int userId))
                    {
                        viewModel.LikedProductIds = await _context.Favorites
                            .Where(f => f.UserId == userId)
                            .Select(f => f.ProductId)
                            .ToListAsync();
                    }
                }

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
                // VÌ SERVICE NÓ ĐÃ XỬ LÝ MAP SANG DTO NÊN CONTROLLER ĐANG THAO TÁC GÓI DỮ LIỆU DTO 
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var relatedProducts = await _productService.GetRelatedProductsAsync(product.CategoryId, product.Id, 8);
                ViewBag.RelatedProducts = relatedProducts;

                // Kiểm tra trạng thái thích
                if (User.Identity.IsAuthenticated)
                {
                    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // XÁC THỰC NGƯỜI DÙNG THÔNG QUA COOKIE 
                    if (int.TryParse(userIdString, out int userId))
                    {
                        product.IsLiked = await _context.Favorites
                            .AnyAsync(f => f.UserId == userId && f.ProductId == product.Id); // CHECK DƯỚI DB XEM LÀ ĐÃ THÍCH HAY CHƯA NẾU CÓ LÀ TRUE ELSE FALSE 

                    }
                }

                return View(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Togglelike(int productId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // XÁC THỰC NGƯỜI DÙNG LOGIN COOKIE 
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này." });
            }

            var productExist = await _productService.GetProductByIdAsync(productId);
            if (productExist == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }
            // sau khi check sản phẩm và user tồn tại 
            var existingLike = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            bool isLiked;
            if (existingLike != null)
            {
                _context.Favorites.Remove(existingLike);
                isLiked = false;
            }
            else
            {
                _context.Favorites.Add(new Favorite { UserId = userId, ProductId = productId });
                isLiked = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, statuslike = isLiked });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveWish(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Chưa đăng nhập." });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == id);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ClearAllWishes()
        {
            // xác thực user tồn tại ( có tài khaorn ) 
            // gọi service xử lý -> xóa hết sp ở favorites cho user này 
            // return lại json succes 
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để sử dụng tính năng này" });
            }
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // XÁC THỰC NGƯỜI DÙNG LOGIN COOKIE 
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này." });
            }
            // goi service để xóa
            await _productService.ClearAllFavoriteAsync(userId);

            return Json(new { success = true, ok = true });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { products = new List<object>() });
            }

            var products = await _productService.SearchQuickAsync(query, 5);
            return Json(new { 
                products = products,
                searchTerm = query
            });
        }
        [HttpPost]
        public async Task<IActionResult> AddReview(ProductReviewDto dto)
        {
            // validation 
            if(!ModelState.IsValid)
            {
                return Json(new { success = false, messsage = " Invalid Input " });
            }
            //  map du lieu qua entity lay nhung thong tin co User da nhap vao -> map xuong entity Model
            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                UserEmail=User.Identity.Name,
                Rating = dto.Rating,
                Title = dto.Title,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now,
                IsVerified = false
            };
            // lưu xuống db 
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            // trả về dữ liệu json 
            return Json(new { success = true ,

                review = new
                { // Trả về object để JS chèn vào UI
                    userName = review.UserEmail,
                    createdAt = review.CreatedAt.ToString("dd/MM/yyyy"),
                    rating = review.Rating,
                    title = review.Title,
                    comment = review.Comment
                }
            });
        }
    }
}
