using AutoMapper;
using Getdata1.Data;
using Getdata1.DTOs;
using Getdata1.Models;
using Getdata1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Getdata1.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context._Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Products)
                        .ThenInclude(p => p.Category)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context._Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Products)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<bool> CreateOrderAsync(int userId, IEnumerable<OrderItemDto> items)
        {
            var order = new _Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = Models.Enums.OrderStatus.Pending,
                // These fields are now required in _Order model, so we need to provide defaults for this legacy method
                FirstName = "System",
                LastName = "User",
                Email = "system@example.com",
                Phone = "0000000000",
                Address = "N/A",
                City = "N/A",
                District = "N/A",
                PaymentMethod = "Unknown",
                ShippingMethod = "Standard",
                OrderItems = items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            _context._Orders.Add(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> CreateOrderFromCartAsync(int userId, CheckoutDto checkoutData, CartDto cart)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new _Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = Models.Enums.OrderStatus.Pending,
                    FirstName = checkoutData.FirstName,
                    LastName = checkoutData.LastName,
                    Email = checkoutData.Email,
                    Phone = checkoutData.Phone,
                    Address = checkoutData.Address,
                    Address2 = checkoutData.Address2,
                    City = checkoutData.City,
                    District = checkoutData.District,
                    ZipCode = checkoutData.ZipCode,
                    OrderNotes = checkoutData.Notes,
                    PaymentMethod = checkoutData.PaymentMethod,
                    ShippingMethod = checkoutData.ShippingMethod,
                    PromoCode = cart.AppliedPromoCode,
                    DiscountAmount = cart.DiscountAmount,
                    // Hardcoded logic for shipping fee based on method in view
                    ShippingFee = checkoutData.ShippingMethod switch
                    {
                        "express" => 30000m,
                        "same-day" => 50000m,
                        _ => 0m
                    }
                };

                _context._Orders.Add(order);
                await _context.SaveChangesAsync(); // Get order ID lưu tậm để lay id 

                foreach (var item in cart.Items)
                {
                    // check lại giá 
                    // SNAPSHOT PRICE: Get current price from database to prevent price manipulation
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null) throw new Exception($"Sản phẩm ID {item.ProductId} không tồn tại.");
                    
                    if (product.Stock < item.Quantity) 
                        throw new Exception($"Sản phẩm {product.Name} không đủ tồn kho.");

                    var orderItem = new OrderItem  // để lưu và vào db 
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price // Snapshot price from DB
                    };

                    product.Stock -= item.Quantity; // Update stock
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}