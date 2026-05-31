using Getdata1.Models;
using Getdata1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<Getdata1.Models.User> _userManager;
        private readonly IOrderService _orderService;

        public ProfileController(UserManager<Getdata1.Models.User> userManager, IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(MyAccount));
        }

        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // xác tnuc nguoi dung base on cookkie
            if (string.IsNullOrEmpty(userIdString)) return Challenge();

            var userId = int.Parse(userIdString);
            var user = await _userManager.FindByIdAsync(userIdString);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            ViewBag.Orders = orders;
            return View(user);
        }
    }
}
