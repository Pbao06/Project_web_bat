using System.ComponentModel.DataAnnotations;

namespace Getdata1.ViewModels
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]





        public string Email { get; set; } = "";




        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]





        public string Password { get; set; } = "";





        [Required(ErrorMessage = "Nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]


        public string ConfirmPassword { get; set; } = "";
        // gom co Email , Pass , Confirmpass 
    }
}
