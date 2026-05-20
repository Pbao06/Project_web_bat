using Getdata1.DTOs;

namespace Getdata1.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<bool> CreateOrderAsync(int userId, IEnumerable<OrderItemDto> items);
        Task<int> CreateOrderFromCartAsync(int userId, CheckoutDto checkoutData, CartDto cart);
    }
}