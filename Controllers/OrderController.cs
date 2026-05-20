using Getdata1.DTOs;
using Getdata1.Helpers;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Getdata1.Controllers
{
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
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();
            if (cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Cart", new { area = "User" });
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = GetCartFromSession();
            if (cart.Items.Count == 0) return RedirectToAction("Index", "Cart", new { area = "User" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var orderItems = cart.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                CategoryName = i.CategoryName,
                Price = i.Price,
                Quantity = i.Quantity
            });

            var success = await _orderService.CreateOrderAsync(userId, orderItems);

            if (success)
            {
                HttpContext.Session.Remove(CartSessionKey);
                return View("Success");
            }

            ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo đơn hàng. Vui lòng thử lại.");
            return View("Checkout", cart);
        }

        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        private CartDto GetCartFromSession()
        {
            return HttpContext.Session.Get<CartDto>(CartSessionKey) ?? new CartDto();
        }
    }
}