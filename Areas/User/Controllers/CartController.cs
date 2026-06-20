using Getdata1.DTOs;
using Getdata1.Models;
using Getdata1.Helpers;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private const string CartSessionKey = "UserCart"; // định nghĩa kho đồ mà user mua 

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
            // SESSION DÙNG ĐỂ => LƯU TẠM THÔNG TIN GIỎ HÀNG
            var cart = HttpContext.Session.Get<CartDto>(CartSessionKey) ?? new CartDto(); // gọi sesion lên 
            // new CartDto nếu session rỗng tự fđọng tạo 
            return Json(cart);
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestedProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                // Lấy ngẫu nhiên khoảng 4 sản phẩm để gợi ý
                var suggested = products.OrderBy(r => Guid.NewGuid()).Take(4).ToList();
                return Json(suggested);
            }
            catch (Exception)
            {
                return Json(new List<ProductDto>());
            }
        }

        //private string GetfinalVariant(ProductDto product,string? ProvidedVariant )
        //{
        //    if (!string.IsNullOrWhiteSpace(ProvidedVariant)) return ProvidedVariant;
        //    var attributes = new List<string>
        //    {
        //        /// đây là những thứ mà khách hàng chọn 
        //        product.Color,product.Weight
        //        ,product.Size,product.Length,
        //    };
        //    return string.Join(" / ", attributes.Where(a => !string.IsNullOrWhiteSpace(a)));
        //}




        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? variant = null)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId); // lấy c9 xác sp từ db check
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                var cart = HttpContext.Session.Get<CartDto>(CartSessionKey) ?? new CartDto();

                // If variant is not provided from UI, we still try to provide some default info
                string finalVariant = variant ?? (!string.IsNullOrEmpty(product.Color) ? $"{product.Color} / {product.Weight}" : product.Weight ?? "");

                // Find existing item with SAME product ID AND SAME variant( variant là về màu sắc , weight , thông tin bé bé để phân biệt 2 món khác nhau tuy cùng loại)
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.VariantInfo == finalVariant);

                if (existingItem != null) // check đảm bảo rằng -> sản phẩn tồn tại 
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Items.Add(new CartItemDto     // vừa theem vào cart và -> map qia CartItemDto để render ra view 
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        CategoryName = product.CategoryName ?? "Sản phẩm",
                        VariantInfo = finalVariant,// tìm mò đúng thông tin chi tiết sp về size ,weight , .. 
                        Price = product.Price,
                        Image = product.Image,
                        Quantity = quantity
                    });
                }


                HttpContext.Session.Set(CartSessionKey, cart); // save lại cho vào cart user
              

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

                        HttpContext.Session.Set(CartSessionKey, cart); // save lại cho vào giỏ user
                    }
                    return Json(new { success = true, cart = cart });
                }
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }

        [HttpGet]
        public IActionResult Checkout() // chuyển sang trang thanh toán 
        {
            if (!User.Identity.IsAuthenticated) // nếu ko có account 
            {
                // Example c) Warning Notification with Login link
                NotificationHelper.SetNotification(TempData, "Bạn cần đăng nhập để thực hiện thanh toán.", "warning", showLoginLink: true);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Payment", "Order");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                var cart = HttpContext.Session.Get<CartDto>(CartSessionKey); // lấy giỏ hàng 
                if (cart != null)
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        cart.Items.Remove(item);
                        HttpContext.Session.Set(CartSessionKey, cart); // lưu vào session 
                    }
                    return Json(new { success = true, cart = cart });
                }
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }
            catch (Exception)
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

            if (promoCode?.ToUpper() == "PBAO2026")
            {
                cart.AppliedPromoCode = "PBAO2026";
                cart.DiscountAmount = 50000; 
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