using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniThrift.Models;
using UniThrift.Data;

namespace UniThrift.Controllers
{
    public class ListingsController : Controller
    {
        private AppDbContext context { get; set;}

        public ListingsController(AppDbContext ctx) =>
            context = ctx;

        // /Listings index view
        public IActionResult Index()
        {
            //Orders by last created
            var listings = context.Listings
                .Include(l=>l.Category)
                .OrderByDescending(
                l=>l.CreatedAt).ToList();
            return View(listings);
        }
        
        // /Listings/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories =  context.Categories.OrderBy(
                c=>c.Name).ToList();
            return View(new Listing());
        }
        // /Listings/Create
        [HttpPost]
        public IActionResult Create(Listing listing)
        {
            // If listing is missing some attributes, reload the categories
            if (!ModelState.IsValid)
            {
                ViewBag.Categories =  context.Categories.OrderBy(
                    c=>c.Name).ToList();
                return View(listing);
            }
            context.Listings.Add(listing);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}

