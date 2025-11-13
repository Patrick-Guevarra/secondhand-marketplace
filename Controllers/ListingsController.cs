using Microsoft.AspNetCore.Mvc;
using UniThrift.Models;

namespace secondhand_marketplace.Controllers
{
    public class ListingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
