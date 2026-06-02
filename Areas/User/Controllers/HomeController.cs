using Getdata1.Data;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;

        public HomeController(IProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            // Lấy danh sách ID sản phẩm đã thích nếu user đã đăng nhập
            if (User.Identity.IsAuthenticated)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int userId))
                {
                    var likedProductIds = await _context.Favorites
                        .Where(f => f.UserId == userId)
                        .Select(f => f.ProductId)
                        .ToListAsync();

                    foreach (var product in products)
                    {
                        product.IsLiked = likedProductIds.Contains(product.Id);
                    }
                }
            }

            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> ContactUs()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> News()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeNewsletter(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Vui lòng nhập địa chỉ email của bạn." });
            }

            // Newsletter should be inclusive. We can check if they have an account just to customize the message,
            // but we shouldn't block them if they don't.
            var userExists = await _context.Users.AnyAsync(u => u.Email == email);

            if (userExists)
            {
                return Json(new { success = true, message = "Chào mừng bạn trở lại! Chúng tôi đã cập nhật đăng ký bản tin cho tài khoản của bạn." });
            }
            
            // For guests, we just acknowledge the subscription.
            // In a real app, we would save this to a NewsletterSubscriptions table.
            return Json(new { success = true, message = "Cảm ơn bạn đã đăng ký! Bạn sẽ nhận được những tin tức mới nhất từ Pbao." });
        }
    }
}
