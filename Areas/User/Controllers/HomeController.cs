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
    }
}
