using Getdata1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Getdata1.Areas.Admin.ViewComponents
{
    public class AdminNotificationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AdminNotificationViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách sản phẩm sắp hết hàng (Stock <= 5)
            var lowStockProducts = await _context.Products
                .Where(p => p.Stock <= 5)
                .OrderBy(p => p.Stock)
                .Take(10) // Giới hạn 10 thông báo mới nhất
                .ToListAsync();

            return View(lowStockProducts);
        }
    }
}
