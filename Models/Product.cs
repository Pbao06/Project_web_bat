using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Đừng quên cái này để dùng IFormFile

namespace Getdata1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        public string? Image {  get; set; }// ? la neu ko co thi null chu dung bao loi 
        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; } // id khóa ngoại phải có nhma nguyên cả đối tượng có thể null vì no ko lấy heeys đc
        [Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int? Stock {  get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Thương hiệu không được để trống")]
        public string? Brand {  get; set; }
        public DateTime? CreatedAt {  get; set; } = DateTime.UtcNow; // no need for nullable

        // Technical Specifications
        public string? Stiffness { get; set; }
        public string? Weight { get; set; }
        public string? Color { get; set; }
        public string? BalancePoint { get; set; }
        public string? Length { get; set; }
        public string? Material { get; set; }
        public string? RecommendedTension { get; set; }
        public string? IncludedString { get; set; }
        public string? GripSize { get; set; }
        public string? Size { get; set; }
        public string? Speed { get; set; }
        public string? Origin { get; set; }
        public string? Warranty { get; set; }

        // 2 thg con của nó là chứa nhieu order item va cart item
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public virtual ICollection<CartItem>? CartItems { get; set; }
        public virtual ICollection<ProductReview>? ProductReviews { get; set; } = new List<ProductReview>();
        // nhung cai nao cho phep null thi de ? de no cho phep null neu ko se bi crash
        [NotMapped]
        public IFormFile? ImageFile { get; set; } // Dùng để upload file main img

        public virtual ICollection<ProductsImage>? ProductImages { get; set; } = new List<ProductsImage>();

        [NotMapped]
        public List<IFormFile>? GalleryFiles { get; set; } // Add the ? to make it optional
        public virtual ICollection<Favorite>? Favorites { get; set; }  // giúp sản phẩm biết đddclike 

    }
}