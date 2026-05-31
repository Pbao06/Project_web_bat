using Getdata1.Areas.User.ViewModels;
using Getdata1.DTOs;
using Getdata1.Helpers;
using Getdata1.Models;
using Getdata1.Models.Enums;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private const string CartSessionKey = "UserCart";

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Payment()
        {
            var cart = HttpContext.Session.Get<CartDto>(CartSessionKey);
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }
            return View("~/Areas/User/Views/Cart/Payment.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutDto checkoutData)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Thông tin không hợp lệ. Vui lòng kiểm tra lại." });
            }

            try
            {
                var cart = HttpContext.Session.Get<CartDto>(CartSessionKey); // lấy giỏ hang từ session ra 
                if (cart == null || !cart.Items.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống." });
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                // 1. Tạo đơn hàng và Snapshot giá trong Service (Transaction included)
                int orderId = await _orderService.CreateOrderFromCartAsync(userId, checkoutData, cart);

                if (orderId > 0)
                {
                    // 2. Xoá giỏ hàng sau khi đặt thành công
                    HttpContext.Session.Remove(CartSessionKey);

                    // 3. Trả về link redirect tới trang Success (bảo mật bằng TempData)
                    TempData["LastOrderId"] = orderId;
                    return Json(new { success = true, redirectUrl = Url.Action("Success", new { id = orderId }) });
                }

                return Json(new { success = false, message = "Không thể tạo đơn hàng." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null || order.UserId != userId)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var lastOrderId = TempData["LastOrderId"] as int?;
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderService.GetOrderByIdAsync(id);
            
            // Security check
            if (order == null || (lastOrderId != id && order.UserId != userId))
            {
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new OrderSuccessVM
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = order.Items,
                SubTotal = order.TotalPrice,
                CustomerName = order.CustomerName ?? "Khách hàng",
                Email = order.Email ?? "",
                Phone = order.Phone ?? "",
                Address = order.Address ?? "",
                PaymentMethod = order.PaymentMethod ?? "N/A",
                ShippingMethod = order.ShippingMethod ?? "N/A",
                ShippingFee = order.ShippingFee,
                DiscountAmount = order.DiscountAmount,
                PromoCode = order.PromoCode
            };

            return View("~/Areas/User/Views/Cart/Order.cshtml", viewModel);
        }
    }
}
