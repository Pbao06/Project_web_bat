using Microsoft.AspNetCore.Mvc;

namespace Getdata1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Product");
        }
    }
}