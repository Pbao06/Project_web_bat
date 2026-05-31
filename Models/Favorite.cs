namespace Getdata1.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } // navigation 
        public DateTime CreateAt { get; set; } = DateTime.Now; // lưu lại giờ mà người dùng bấm thích phòng trường hợp sau này lọc theo time
    }
}
