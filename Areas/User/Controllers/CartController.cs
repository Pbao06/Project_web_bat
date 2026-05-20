using Getdata1.DTOs;
using Getdata1.Helpers;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private const string CartSessionKey = "UserCart";

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var cart = HttpContext.Session.Get<CartDto>(CartSessionKey) ?? new CartDto();
            return Json(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                var cart = HttpContext.Session.Get<CartDto>(CartSessionKey) ?? new CartDto();
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Items.Add(new CartItemDto
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        CategoryName = product.CategoryName ?? "Sản phẩm",
                        VariantInfo = !string.IsNullOrEmpty(product.Color) ? $"{product.Color} / {product.Weight}" : product.Weight,
                        Price = product.Price,
                        Image = product.Image,
                        Quantity = quantity
                    });
                }

                HttpContext.Session.Set(CartSessionKey, cart);
                    
                return Json(new { 
                    success = true, 
                    message = $"Đã thêm {product.Name} vào giỏ hàng!",
                    cartCount = cart.Items.Sum(i => i.Quantity),
                    cart = cart
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm vào giỏ hàng." });
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var cart = HttpContext.Session.Get<CartDto>(CartSessionKey);
                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        if (quantity <= 0)
                        {
                            cart.Items.Remove(item);
                        }
                        else
                        {
                            item.Quantity = quantity;
                        }
                        HttpContext.Session.Set(CartSessionKey, cart);
                    }
                    return Json(new { success = true, cart = cart });
                }
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                var cart = HttpContext.Session.Get<CartDto>(CartSessionKey);
                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        cart.Items.Remove(item);
                        HttpContext.Session.Set(CartSessionKey, cart);
                    }
                    return Json(new { success = true, cart = cart });
                }
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ApplyPromo(string promoCode)
        {
            var cart = HttpContext.Session.Get<CartDto>(CartSessionKey);
            if (cart == null || !cart.Items.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }

            // Mock promo logic
            if (promoCode?.ToUpper() == "PBAO2026")
            {
                cart.AppliedPromoCode = "PBAO2026";
                cart.DiscountAmount = 23; // Fixed discount for demo
                HttpContext.Session.Set(CartSessionKey, cart);
                return Json(new { success = true, message = "Áp dụng mã giảm giá thành công!", cart = cart });
            }

            return Json(new { success = false, message = "Mã giảm giá không hợp lệ." });
        }
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            return View();
        }
    }
}
