namespace Getdata1.DTOs
{
    public class Top1ProductDTO
    {
        // Name method là phải trùng với Db để nó biết đường mà tự liên kết 
        
        public int? ProductId { get; set; }
        public string? ProductName { get; set; } 
        public string? Brand {  get; set; }
        // vì lấy top 1 bán chạy nên ta lấy Số lượng + price của Order 
        public decimal TotalPrice { get; set; }
        public int TotalSold { get; set; }


    }
}
