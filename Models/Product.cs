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
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }

        public string? Image {  get; set; }// ? la neu ko co thi null chu dung bao loi 
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; } // id khóa ngoại phải có nhma nguyên cả đối tượng có thể null vì no ko lấy heeys đc
        [Required]
        public int Stock {  get; set; }
        public string? Description { get; set; }
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

    }
}