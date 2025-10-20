using Microsoft.AspNetCore.Mvc;
using secondhand_marketplace.Models;

namespace secondhand_marketplace.Controllers
{
    public class ListingsController : Controller
    {
        public IActionResult Index()
        {

            var listings = new List<Listings>
            {
                new Listings { Title = "Nike Hoodie", Price = 30, Category = "Clothing", Description = "Lightly worn", ImageURL = "/images/nikehoodie.jpg"},
                new Listings { Title = "TI-84 Plus CE", Price = 40, Category = "Electronics", Description = "Works perfectly", ImageURL = "/images/ti84.jpg" },
                new Listings { Title = "Desk Lamp", Price = 10, Category = "Home", Description = "Works perfectly", ImageURL = "/images/lamp.jpg" },
                new Listings { Title = "Nike Hoodie", Price = 30, Category = "Clothing", Description = "Lightly worn", ImageURL = "/images/nikehoodie.jpg"},
                new Listings { Title = "TI-84 Plus CE", Price = 40, Category = "Electronics", Description = "Works perfectly", ImageURL = "/images/ti84.jpg" },
                new Listings { Title = "Desk Lamp", Price = 10, Category = "Home", Description = "Works perfectly", ImageURL = "/images/lamp.jpg" }
            };

            return View(listings);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
