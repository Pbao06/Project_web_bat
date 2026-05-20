using System.ComponentModel.DataAnnotations;

namespace Getdata1.DTOs
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "Vui lòng nhập họ.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string Address { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành phố.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện.")]
        public string District { get; set; }

        public string? ZipCode { get; set; }

        public string? Notes { get; set; }

        [Required]
        public string ShippingMethod { get; set; }

        [Required]
        public string PaymentMethod { get; set; }
        
        public string? PromoCode { get; set; }
    }
}
