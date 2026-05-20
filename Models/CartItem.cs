using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Getdata1.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int CartId {  get; set; }

        public virtual Cart? Cart { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Products { get; set; }
        public int Quantity {  get; set; }
        public decimal Price { get; set; }
    }
}
