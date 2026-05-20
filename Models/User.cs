using Getdata1.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Getdata1.Models
{
    public class User : IdentityUser<int>
    {
        public UserRole Role { get; set; } = UserRole.Customer; // default to Customer
        public virtual ICollection<_Order> Orders { get; set; } = new List<_Order>();
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
