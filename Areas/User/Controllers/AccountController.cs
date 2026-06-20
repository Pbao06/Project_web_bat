using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Getdata1.Models;
using Getdata1.Models.Enums;
using Getdata1.ViewModels;
using Getdata1.Helpers;

namespace Getdata1.Areas.User.Controllers
{
    [Area("User")]
    public class AccountController : Controller
    {
        
        private readonly SignInManager<Getdata1.Models.User> _signInManager;
        private readonly UserManager<Getdata1.Models.User> _userManager;

        public AccountController(SignInManager<Getdata1.Models.User> signInManager,
                                  UserManager<Getdata1.Models.User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                NotificationHelper.SetNotification(TempData, "Vui lòng nhập đầy đủ thông tin đăng nhập.", "error");
                return View(model);
            }

            var user = await _signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,lockoutOnFailure:false);
            
            if (user.Succeeded)
            {
                var data = await _userManager.FindByEmailAsync(model.Email);
                
                // Example a) Success Notification
                NotificationHelper.SetNotification(TempData, $"Chào mừng {data?.Email} đã quay trở lại!", "success");
                if(data!=null && await _userManager.IsInRoleAsync(data,"Admin"))
                {
                   return  RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return  RedirectToAction("Index", "Home", new { area = "User" });

              
            }

            // Example b) Error Notification
            NotificationHelper.SetNotification(TempData, "Tài khoản hoặc mật khẩu không chính xác.", "error");
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");
            ModelState.AddModelError("Email", "");
            ModelState.AddModelError("Password", "");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotificationHelper.SetNotification(TempData, "Vui lòng nhập đầy đủ thông tin đăng ký.", "error");
                    return View(model);
                }

                var getdata = await _userManager.FindByEmailAsync(model.Email);
                if (getdata != null)
                {
                    NotificationHelper.SetNotification(TempData, "Email này đã được sử dụng.", "error");
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                var user = new Getdata1.Models.User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Role = UserRole.Customer
                };

                var save = await _userManager.CreateAsync(user, model.Password); // auto băm pass trong service func này r 
                
                if (save.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    NotificationHelper.SetNotification(TempData, "Đăng ký tài khoản thành công!", "success");
                    return RedirectToAction("Index", "Home");
                }

                bool hasPasswordError = false;
                foreach (var error in save.Errors)
                {
                    if (error.Code.Contains("Password")) hasPasswordError = true;
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                if (hasPasswordError)
                {
                    NotificationHelper.SetNotification(TempData, "Mật khẩu không đủ mạnh.", "error");
                }
                else
                {
                    NotificationHelper.SetNotification(TempData, "Đăng ký không thành công. Vui lòng kiểm tra lại.", "error");
                }
            }
            catch (Exception)
            {
                NotificationHelper.SetNotification(TempData, "Đã xảy ra lỗi hệ thống.", "error");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi hệ thống trong quá trình đăng ký.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            NotificationHelper.SetNotification(TempData, "Bạn đã đăng xuất thành công.", "success");
            return RedirectToAction("Login", "Account", new { area = "User" });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}