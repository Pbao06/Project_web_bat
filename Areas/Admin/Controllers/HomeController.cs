using AspNetCoreGeneratedDocument;
using Getdata1.Data;
using Getdata1.Models;
using Getdata1.Models.Enums;
//using Getdata1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Getdata1.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Getdata1.Areas.Admin.Controllers
{
    //THIS FOR DASHBOARD MAIN PROJECT ADMIN 
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult> Index(string? status)
        {
            try
            {
                var query = _context._Orders.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
                    {
                        query = query.Where(o => o.Status == orderStatus);
                    }
                }

                var viewmodel = new DashboardVM
                {
                    TotalCustomer = await _context.Users.CountAsync(),
                    TotalOrders = await _context._Orders.CountAsync(),
                    PendingOrders = await _context._Orders.Where(o => o.Status == OrderStatus.Pending).CountAsync(),
                    RecentOrders = await query.Include(o => o.User).OrderByDescending(o => o.OrderDate).Take(5).ToListAsync(),
                    LowStockProducts = await _context.Products.Where(p => p.Stock <= 5).ToListAsync(),
                    TotalSales = await _context._Orders
                    .Where(o => o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Paid)
                    .SelectMany(o => o.OrderItems) // Trải phẳng tất cả các món hàng của các đơn hàng đã lọc
                    .SumAsync(item => item.Price * item.Quantity) // Tính tổng dựa trên giá và số lượng từng món
                };

                ViewBag.Status = status;
                return View(viewmodel);
            }

            catch (Exception ex)
            {
                return Content("Error: " + ex.Message + " | Inner: " + ex.InnerException?.Message);
            }

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
