using Getdata1.Data;
using Getdata1.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Getdata1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")] // set chi co admin moi duoc vao 
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context) // ocnstructor 
        {
            _context = context;
        }
        public  async Task<IActionResult> Index(int page=1)
        {
            // get data -> checknull ->  maps -> return 
            // in this case we maps direcly 
          try
            {
                int pageSize = 10;
                var query = _context.Users.AsQueryable();
                int TotalItems = await query.CountAsync();




                var CustomerData =await query.Select(o => new CustomerVM
                {
                    Name = o.UserName,
                    PasswordHash = o.PasswordHash,
                    OrderCount = _context._Orders.Count(u => u.UserId == o.Id),
                    TotalSpent = _context._Orders.Where(u => u.UserId == o.Id).SelectMany(o => o.OrderItems).Sum(item => (decimal?)(item.Price * item.Quantity)) ?? 0,
                    // cause TotalPrice notmap in oject so we have to -> calculate base join real o
                    Status = (_context._Orders
                    .Where(u => u.UserId == o.Id)
                    .SelectMany(ord => ord.OrderItems)
                    .Sum(item => (decimal?)(item.Price * item.Quantity)) ?? 0) >= 500000
                    ? "VIP" : "Regular"
                }).OrderByDescending(o => o.TotalSpent).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                if (CustomerData == null) return NotFound();
                // maps to customerIndexData
                var model = new CustomerIndexVM
                {
                    Customers = CustomerData,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(TotalItems / (double)pageSize)
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
