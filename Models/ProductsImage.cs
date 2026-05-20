using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Getdata1.Models
{
    [Table("ProductsImages")] // This maps your C# class to the DB table exactly
    public class ProductsImage
    {
        [Key]
        public int Id { get;set;  }
        public string? Url { get; set; }       
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

    }
}
