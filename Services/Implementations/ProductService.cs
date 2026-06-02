using AutoMapper;
using Getdata1.Data;
using Getdata1.DTOs;
using Getdata1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Getdata1.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
            
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {       
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            // vì qua để lấy sản phẩm vào sesssion nên ở đây không cần join vào productReview cho nó lâu 
            return _mapper.Map<ProductDto>(product); // tự động map thông tin qua Product DTO mà ko cần viết thủ công 
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetFilteredProductsAsync(
            int? categoryId, string? searchTerm, string? brand, decimal? minPrice, decimal? maxPrice, string? sortBy, int page, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();// đây là cần thiết cho 1 thông tin sản phẩm 

            // Filter by Category
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Filter by Brand
            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand != null && p.Brand.ToLower() == brand.ToLower());
            }

            // Filter by Search Term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || (p.Brand != null && p.Brand.Contains(searchTerm)));
            }

            // Filter by Price Range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "latest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products); // trả về product -> DTO 
            return (dtos, totalCount);
        }

        public async Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int categoryId, int currentProductId, int count = 4)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId && p.Id != currentProductId)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .OrderBy(r => Guid.NewGuid()) // Random order
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        public async Task<List<ProductDto>> GetWistlistByUserIdAsync(int userId)
        {
            // lấy danh sách product ra từ favorites của user truyền vào parameter
            var favoriteProducts = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product).ThenInclude(p => p.ProductImages)
                .Include(f => f.Product).ThenInclude(p => p.Category)
                .Select(f => f.Product)
                .ToListAsync();
            return _mapper.Map<List<ProductDto>>(favoriteProducts);

        }
        public async Task<bool> ClearAllFavoriteAsync(int userId)
        {
            // tìm tất cả sản phẩm của user
            var AllUserFavorite = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync(); // lấy tất cả sản phẩm trong Favorite của user
            if(AllUserFavorite.Any())
            {
                _context.Favorites.RemoveRange(AllUserFavorite);
                await _context.SaveChangesAsync(); // lưu về db 
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ProductDto>> SearchQuickAsync(string query, int limit = 5)
        {
            if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<ProductDto>();

            var products = await _context.Products
                .Where(p => p.Name.Contains(query) || (p.Brand != null && p.Brand.Contains(query)))
                .Include(p => p.ProductImages)
                .Take(limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        // lấy sản phẩm bán chạy nhất 
        public async Task<Top1ProductDTO> GetTop1Product()
        {
            // thg loz Dto chỉ là nơi để hứng và xử lý dữ liệu  đựng tạm  -> thg model là cái khung 
            // DATA SQL LÀ CỤC ĐẤT SÉT => MƯỢN KHUNG DTO -> NẶN RA HÌNH HÀI -> LẤY ĐẤT ĐÃ NẶN RA KHỎI KHUNG TRẢ VỀ CHO CONTROLLER 
            // Nhịp 1: Chạy lệnh EXEC nguyên thủy, không chế cháo, và bưng trọn gói về RAM (dùng ToListAsync)
            var listResult = await _context.Database
                .SqlQuery<Top1ProductDTO>($"EXEC sp_Laytop1sanpham")
                .ToListAsync();

            // Nhịp 2: Lúc này data đã nằm an toàn trên RAM của C#. Mình thoải mái thò tay vào lấy thằng đầu tiên ra!
            var topProduct = listResult.FirstOrDefault();
            return topProduct;
        }
        // Hàm đếm số lượng sản phẩm đang bị outstock
        public async Task<int> Laylowstock()
        {
            int lowstock = await _context.Products.Where(p => p.Stock <= 10).CountAsync();
            return lowstock;
        }





    }
}