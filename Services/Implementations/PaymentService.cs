using Getdata1.Areas.Admin.ViewModels;
using Getdata1.Data;
using Getdata1.Models;
using Getdata1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Getdata1.Services.Implementations
{
    public class PaymentService: IPaymentService
    {
        private readonly ApplicationDbContext _context;
        //public PaymentService(ApplicationDbContext context)=> _context= context;// constructor init 
        private readonly IConfiguration _config;
   
        public PaymentService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        //public string CreatePaymentUrlAsync(int OrderId,HttpContext httpContext)
        //{
        //    // lay so luong tu order 
        //    var order = _context._Orders.FirstOrDefault(o=>o.Id==OrderId);
        //    if (order == null) throw new Exception(" Not found Order");

        //    // get dia chi aspnet core
        //    // ✅ Giữ nguyên 3 cái này, không bỏ
        //    // Ép cứng IPV4 để tránh lỗi ::1
        //    string idAddress = "127.0.0.1";

        //    string tmnCode = _config["VnPay:TmnCode"];
        //    string hashSecret = _config["VnPay:HashSecret"]?.Trim() ?? "";
        //    string baseUrl = _config["VnPay:BaseUrl"];

        //    var pay = new VnPayLibrary();
        //    pay.AddRequestData("vnp_Version", "2.1.0");
        //    pay.AddRequestData("vnp_Command", "pay");
        //    pay.AddRequestData("vnp_TmnCode", tmnCode);
        //    pay.AddRequestData("vnp_Amount", ((long)(order.TotalPrice * 100)).ToString()); // VNPay yêu cầu * 100
        //    pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    pay.AddRequestData("vnp_CurrCode", "VND");
        //    pay.AddRequestData("vnp_IpAddr", idAddress); // truyen id vao 
        //    pay.AddRequestData("vnp_Locale", "vn");
        //    pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {OrderId}");
        //    pay.AddRequestData("vnp_OrderType", "other");
        //    pay.AddRequestData("vnp_ReturnUrl", "https://localhost:7033/Order/PaymentCallback"); // Link trả về

        //    //pay.AddRequestData("vnp_TxnRef", OrderId.ToString()); // Mã đơn hàng duy nhất
        //    // 
        //    // Mã đơn duy nhất 
        //    string txnRef = OrderId.ToString() + DateTime.Now.Ticks.ToString();
        //    pay.AddRequestData("vnp_TxnRef", txnRef);

        //    // 2. Tạo chữ ký bảo mật (Signature)
        //    //string signData = pay.GetRequestData(); // Lấy chuỗi đã ghép để debug
        //    //Console.WriteLine("DATA SIGN: " + signData); // In ra cửa sổ Output của Visual Studo


        //    // 2. Tạo chữ ký bảo mật (Signature) - Đây là bước quan trọng nhất
        //    return pay.CreateRequestUrl(baseUrl, hashSecret);
        //}

        public async Task<string> CreatePaymentUrlAsync(int OrderId, HttpContext httpContext)
        {
            var order= await _context._Orders.FirstOrDefaultAsync(p=>p.Id==OrderId);
            // Kiểm tra xem đơn hàng có tồn tại không
            if (order == null) throw new Exception("Không tìm thấy đơn hàng!");
            string tmnCode = _config["VnPay:TmnCode"];
            string hashSecret = _config["VnPay:HashSecret"]?.Trim() ?? "";
            string baseUrl = _config["VnPay:BaseUrl"];
           
            // 1. Dùng SortedDictionary để tự động sắp xếp theo Alphabet (A-Z)
            var dict = new SortedDictionary<string, string>{
        { "vnp_Amount", ((long)(order!.TotalPrice * 100)).ToString() },
        { "vnp_Command", "pay" },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "vnp_CurrCode", "VND" },
        { "vnp_IpAddr", "127.0.0.1" }, // Nên lấy IP thực tế của khách hàng
        { "vnp_Locale", "vn" },
        { "vnp_OrderInfo", "Thanhtoandonhang:" + OrderId },
        { "vnp_OrderType", "other" },
        { "vnp_ReturnUrl", "https://matador-travel-cost.ngrok-free.dev/Order/PaymentCallback" },
        { "vnp_TmnCode", tmnCode },
        { "vnp_TxnRef", OrderId.ToString() }, // Bỏ Ticks đi cho dễ đối soát
        { "vnp_Version", "2.1.0" }
    };

            // 2. Tạo chuỗi rawSignData KHÔNG Encode
            var rawSignData = new StringBuilder();
            foreach (var entry in dict)
            {
                rawSignData.Append(entry.Key + "=" + entry.Value + "&");
            }
            // Xóa dấu & cuối cùng
            rawSignData.Remove(rawSignData.Length - 1, 1);
            // debug
            Console.WriteLine(rawSignData.ToString());

            // 3. Băm HMACSHA512
            string vnp_SecureHash;
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawSignData.ToString()));
                vnp_SecureHash = BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }

            // 4. Tạo URL gửi đi (Phải UrlEncode giá trị)
            // 4. Tạo URL gửi đi
            var queryString = new StringBuilder();
            foreach (var entry in dict)
            {
                queryString.Append(WebUtility.UrlEncode(entry.Key) + "=" + WebUtility.UrlEncode(entry.Value) + "&");
            }

            // BỎ DẤU & CUỐI CÙNG TRƯỚC KHI THÊM HASH
            if (queryString.Length > 0)
            {
                queryString.Remove(queryString.Length - 1, 1);
            }

            return $"{baseUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
        }
    }
}
