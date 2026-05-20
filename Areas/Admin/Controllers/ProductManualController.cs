using Getdata1.Areas.Admin.ViewModels;
using Getdata1.Data;
using Getdata1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Getdata1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // kich hoat chi co admin moi duoc sai kk 
    public class ProductManualController : Controller
    {
        private readonly ApplicationDbContext _context; // dinh nghia db 
        private readonly IWebHostEnvironment _webHostEnvironment; // THÊM DÒNG NÀY
        public ProductManualController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
       
        }
        [HttpGet]
        public async Task<IActionResult> Index(ProductFilterVM filter, int page = 1)
        {
            try
            {
                // 1. Get query (AsQueryable means we haven't hit the DB yet)
                var query = _context.Products.Include(p => p.Category).Include(p=>p.ProductImages).AsQueryable();

                // 2. Apply Filters
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    query = query.Where(p => p.Name.Contains(filter.Name));
                }
                if (filter.CategoryId != null)
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId);
                }
                if (filter.MinPrice != null)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice);
                }
                if (filter.MaxPrice != null)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice);
                }
                if (filter.MinStock != null)
                {
                    query = query.Where(p => p.Stock >= filter.MinStock);
                }
                if (filter.MaxStock != null)
                {
                    query = query.Where(p => p.Stock <= filter.MaxStock);
                }

                // 3. Execution & Pagination
                // Use CountAsync to keep the whole method truly asynchronous
                int totalItems = await query.CountAsync();
                int pageSize = 10;

                // Skip/Take for pagination
                var rawProducts = await query
                    .OrderByDescending(p => p.Price)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 4. Map to VM & Handle empty results
                filter.Products = rawProducts ?? new List<Product>();
                filter.CurrentPage = page;
                filter.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // 5. Fill ViewBag (The fix for your "InvalidOperation" bug)
                var categories = await _context.Categories.ToListAsync();
                ViewBag.Category = new SelectList(categories, "Id", "Name",  filter.CategoryId);

                return View(filter);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // Create 
        [HttpGet]
        public IActionResult Create()
        {
            // mở trang create để fix vì nó để mở ra nên ko cần async (ta cần nó chạy xong mới thực hiện cía khác)
            ViewData["Category"] = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.CreatedAt = DateTime.Now;
            // Bỏ qua lỗi validation của ProductImages vì lúc này nó chưa có dữ liệu
            ModelState.Remove("ProductImages");
            if (ModelState.IsValid)
            {
                // 1. Lưu ảnh chính (Main Image)
                if (product.ImageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", fileName); // path to folder
                    using (var stream = new FileStream(path, FileMode.Create)) { await product.ImageFile.CopyToAsync(stream); }
                    product.Image = "/img/" + fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync(); // Lưu để lấy Product.Id

                // 2. Lưu danh sách ảnh Gallery (GalleryFiles) - ĐÂY LÀ CHỖ QUAN TRỌNG
                if (product.GalleryFiles != null && product.GalleryFiles.Any())
                {
                    foreach (var file in product.GalleryFiles)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", fileName);

                        using (var stream = new FileStream(path, FileMode.Create)) { await file.CopyToAsync(stream); }

                        // Tạo dòng mới cho bảng ProductsImages
                        var newImage = new ProductsImage
                        {
                            ProductId = product.Id, // Gán ID của Product vừa tạo
                            Url = "/img/" + fileName
                        };
                        _context.ProductsImages.Add(newImage);
                    }
                    await _context.SaveChangesAsync(); // Lưu tất cả ảnh vào SQL
                }
             
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // ĐOẠN NÀY SẼ NÓI CHO BẠN BIẾT TẠI SAO NÓ KHÔNG CHẠY
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine("LỖI VALIDATION: " + error.ErrorMessage);
                }
            }
            ViewBag.Category = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        // Edit ;
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(id==0)
            {
                return NotFound();
            }
            var sp = await _context.Products.Include(m => m.Category).Include(i=>i.ProductImages).FirstOrDefaultAsync(p => p.Id == id); // lay sp
            if (sp== null) return NotFound(); // rule get xong check null then return 
            ViewBag.Category = new SelectList(_context.Set<Category>(), "Id", "Name");
            return View(sp);
        }
        // Post edit 
        public bool ProductExist(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  // Fix 4: prevent CSRF attacks

        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            // Xóa sạch lỗi validation ngầm để chắc chắn nhảy vào trong
            ModelState.Remove("Category");
            ModelState.Remove("ProductImages");
            ModelState.Remove("OrderItems");
            ModelState.Remove("CartItems");

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Xử lý Ảnh chính mới (Nếu người dùng upload ảnh mới)
                    if (product.ImageFile != null)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                        //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", fileName); // path to folder
                        using (var stream = new FileStream(path, FileMode.Create)) { await product.ImageFile.CopyToAsync(stream); }
                        product.Image = "/img/" + fileName;
                    }
                    else
                    {
                        // Nếu không upload ảnh mới, giữ nguyên ảnh cũ (tránh bị null)
                        _context.Entry(product).Property(x => x.Image).IsModified = false;
                    }

                    // 2. Cập nhật thông tin Product
                    _context.Update(product);
                    // Không cập nhật ngày tạo khi Edit
                    _context.Entry(product).Property(x => x.CreatedAt).IsModified = false;
                    await _context.SaveChangesAsync();

                    // 3. Xử lý thêm ảnh Gallery mới (Nếu có chọn thêm file)
                    if (product.GalleryFiles != null && product.GalleryFiles.Any())
                    {
                        foreach (var file in product.GalleryFiles)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", fileName);
                            using (var stream = new FileStream(path, FileMode.Create)) { await file.CopyToAsync(stream); }

                            var newImage = new ProductsImage
                            {
                                ProductId = product.Id,
                                Url = "/img/" + fileName
                            };
                            _context.ProductsImages.Add(newImage);
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi: " + ex.Message);
                }
            }
            
            ViewBag.Category = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Delete(int id,Product product)
        {
            if(id!=product.Id)
            {
                return NotFound();
            }
            // nếu có Id rồi 
            var sp = await _context.Products.Include(m => m.Category).Include(m=>m.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            // vì ko cho sửa category nên ta sẽ không sửa 
            if(sp==null)
            {
                return NotFound();
            }
            return View(sp); 
        }
        // post 
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken] // lớp áo giáp bảo vệ 
        public async Task<IActionResult> DeleteConfrimed(int id)
        {
            var sp = await _context.Products.FindAsync(id); // tìm sản phẩm dựa trên id 
            if(sp!=null)
            {
                _context.Products.Remove(sp);

            }    
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            // get data form id Specific
            if (id == null)
            {
                return NotFound();
            }
            var data = await _context.Products.Include(_ => _.Category).Include(_ => _.ProductImages).FirstOrDefaultAsync(m => m.Id == id);
            if (data == null) return NotFound();
            return View(data);
        }



        [HttpPost]
        [IgnoreAntiforgeryToken] // Thêm dòng này để bỏ qua kiểm tra token khi gọi AJAX

        public async Task<IActionResult> DeleteImage(int id)
        {
            var img = await _context.ProductsImages.FindAsync(id);
            if (img == null) return Json(new { success = false, message = "Không tìm thấy ảnh" });

            try
            {
                var filepath = Path.Combine(_webHostEnvironment.WebRootPath, img.Url.TrimStart('/'));
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                _context.ProductsImages.Remove(img);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

    }
}
