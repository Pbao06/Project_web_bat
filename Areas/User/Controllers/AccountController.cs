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
            if (!ModelState.IsValid) return View(model);

            var user = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            
            if (user.Succeeded)
            {
                var data = await _userManager.FindByEmailAsync(model.Email);
                
                // Example a) Success Notification
                NotificationHelper.SetNotification(TempData, $"Chào mừng {data?.Email} đã quay trở lại!", "success");

                if (data != null && await _userManager.IsInRoleAsync(data, "Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            // Example b) Error Notification
            NotificationHelper.SetNotification(TempData, "Tài khoản hoặc mật khẩu không chính xác.", "error");
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");
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
                if (!ModelState.IsValid) return View(model);

                var getdata = await _userManager.FindByEmailAsync(model.Email);
                if (getdata != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                var user = new Getdata1.Models.User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Role = UserRole.Customer
                };

                var save = await _userManager.CreateAsync(user, model.Password);
                
                if (save.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    NotificationHelper.SetNotification(TempData, "Đăng ký tài khoản thành công!", "success");
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in save.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception)
            {
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