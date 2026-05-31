using Getdata1.Data;
using Getdata1.Models;
using Getdata1.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Getdata1.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Getdata1.Helpers;

namespace Getdata1.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles ="Admin")]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _context; // khai bao db
    //constructor 
    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }
    // Get: /Order/Index
    public async Task<IActionResult> Index(string? status,int page=1)
    {
        try
        {
            int pageSize = 10;
            // Step 1 - build query
            var query = _context._Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Products)
                .AsQueryable();

            // Step 2 - apply filter
            if (!string.IsNullOrEmpty(status))
                if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
                    query = query.Where(o => o.Status == orderStatus);

            int totalOrders = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);
            // Step 3 - execute query
            var rawOrders = await query
                .OrderByDescending(o => o.OrderDate).Skip((page-1)*pageSize).Take(pageSize)
                .ToListAsync();

            // Step 4 - map to VM
            var orders = rawOrders.Select(o => new OrderIndexVM
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                UserName = o.User != null ? o.User.UserName : "N/A",
                Status = o.Status,
                TotalPrice = o.OrderItems != null
                             ? o.OrderItems.Sum(i => i.Price * i.Quantity)
                             : 0
            }).ToList();
            // map to orderPage
            var vm = new OrderPageVM
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = totalPages,
                Status = status
            };

            ViewBag.Status = status;
            return View(vm);
        }
        catch (Exception ex)
        {
            return Content("Error: " + ex.Message + " | Inner: " + ex.InnerException?.Message);

        }
    }
    // get product detail
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var raw = await _context._Orders.Include(p => p.User).Include(p => p.OrderItems).ThenInclude(i => i.Products).FirstOrDefaultAsync(p => p.Id == id);
            if (raw == null) return NotFound();
            // map to VM 
            var order = new OrderVM
            {
                OrderId = raw.Id, // right is real data 
                UserName = raw.User != null ? raw.User.UserName : "N/A",
                OrderDate = raw.OrderDate,
                Status = raw.Status,
                Items = raw.OrderItems.Select(i => new OrderItemVM
                {
                    //right side is real data in db 
                    ProductName = i.Products != null ? i.Products.Name : "N/A",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
            return View(order);
        }
        catch (Exception ex)
        {
            return Content("Error:" + ex.Message + " | Inner:" + ex.InnerException?.Message);
        }
    }
    public async Task<IActionResult> UpdateStatus(int id,string status)
    {
        var order = await _context._Orders.FindAsync(id);
        if (order == null)
        {
            NotificationHelper.SetNotification(TempData, "Không tìm thấy đơn hàng.", "error");
            return NotFound();
        }

        // convert string to enum
        if(Enum.TryParse<OrderStatus>(status,out var newStatus))
        {
            order.Status=newStatus;
            await _context.SaveChangesAsync();
            
            string message = newStatus == OrderStatus.Paid ? "Đã duyệt đơn hàng thành công!" : "Đã hủy đơn hàng.";
            string type = newStatus == OrderStatus.Paid ? "success" : "warning";
            NotificationHelper.SetNotification(TempData, message, type);
        }
        else
        {
            NotificationHelper.SetNotification(TempData, "Trạng thái không hợp lệ.", "error");
        }
        return RedirectToAction("Index");
    }
}
