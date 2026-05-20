using System.ComponentModel.DataAnnotations;

namespace Getdata1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        // nhieu san pham thuoc 1 category
        public virtual ICollection<Product>? Products { get; set; } // mqh 1 nhiều

    }
}
