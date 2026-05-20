using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Getdata1.Models;
using Getdata1.Models.Enums;
using Getdata1.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]

    public class AccountController : Controller
    {
        // readonly = just use for this file cannot fix after did 
         private readonly SignInManager<Getdata1.Models.User> _signInManager; //handle login /register
        
        private readonly UserManager<Getdata1.Models.User> _userManager;//handle manager user 
        // constructor 
        public AccountController(SignInManager<Getdata1.Models.User> signInManager,
                                  UserManager<Getdata1.Models.User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
         [HttpGet]
        public IActionResult Login() => View(); // mo trang dang nhap 



         [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // flow -> check ModelStade -> check exits under Db -> ge email compare to email admin if == admin redirect to admin 
            if(!ModelState.IsValid) return View(model);
            // under code -> check if exist in db 
            var user = await _signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,lockoutOnFailure:false);
            // lockoutOnFailure -> if user nhập quá nhiều sẽ bị lock 
            if(user.Succeeded)
            {
                var data = await _userManager.FindByEmailAsync(model.Email);
                if(data != null && await _userManager.IsInRoleAsync(data,"Admin")) // compare if == email admin 
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                        /// structure of RedirectToAction is 
                }
                // if not admin => return for Home stoe 
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
            ModelState.AddModelError(String.Empty, "This sentences not illigal could you login again ");
            return View(model);
        }


         [HttpGet]
         // use for open view 
        public IActionResult Register() => View();



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            /* 
             * CHECK FLOW 
             * KIỂM TRA MODELSTADE XEM NHẬP ĐỦ CHƯA 
             * LẤY VÀ KIỂM TRA TỒN TẠI -> NẾU CHƯA CÓ MỚI CHO ĐĂNG NHẬP 
             * TẠO NEW USER -> FILL OUT DATA VIEWMODEL VÀO -> LƯU , 
             * CHECK SAVE THÀNH CÔNG THÌ GÁN ROLE LÀ CUSTOMER -> CHO PHÉP ĐĂNG NHẬP TRỰC TIẾP
             * -> REDIRECT TO ACTION 
             
            
             */
            try
            {
                if (!ModelState.IsValid) return View(model);
                // get du lieu check ton tai 
                var getdata = await _userManager.FindByEmailAsync(model.Email);
                if (getdata != null)
                {
                    ModelState.AddModelError("Email", " This email was exit could u log in ? ");
                    return View(model);
                }
                // net khong ton tai cho dang ki tiep 
                // tạo new tài khoản mới = cách create new user, set role = customer 
                var user = new Getdata1.Models.User
                {
                    Email = model.Email,
                    UserName=model.Email,
                    Role=UserRole.Customer
                };
                // map roi save 

                var save = await _userManager.CreateAsync(user, model.Password);// user ,pass phải đi kèm để nhận diện cho hàm băm password cấp cho salt riêng để băm 
                
                
          
       
    
            }
            catch (Exception ex)
            {
                // 8. Xử lý ngoại lệ hệ thống hoặc lỗi kết nối Database
                ModelState.AddModelError("Email", "Đã xảy ra lỗi hệ thống trong quá trình đăng ký. Vui lòng thử lại sau.");
                // Log lỗi ex ở đây nếu cần thiết
            }

            return View(model);
        }


        // [HttpPost]
        //[ValidateAntiForgeryToken] -> vì sử dụng thẻ a 

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // xóa cookie user 
            return RedirectToAction("Login", "Account", new { area = "User" });
            // gọi view -> login , controller Account , areas User 
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
