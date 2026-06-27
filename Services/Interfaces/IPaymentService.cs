namespace Getdata1.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrlAsync(int OrderId, HttpContext httpContext);
    }
}
