using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Getdata1.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        // defind userid foreign key of User
        public virtual User User { get; set; }
        public DateTime CreatedAt { get; set; }
        // vi cart co the chua nhieu cart item
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
