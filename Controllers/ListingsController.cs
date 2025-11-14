using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniThrift.Models;
using UniThrift.Data;

namespace UniThrift.Controllers
{
    public class ListingsController : Controller
    {
        private AppDbContext context { get; set;}
        private readonly IWebHostEnvironment env; 

        public ListingsController(AppDbContext ctx, IWebHostEnvironment env)
        {
            context = ctx;
            this.env = env;
        }
            

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
        // Listings/Create
        [HttpPost]
        public async Task<IActionResult> Create(Listing listing) //Uploading files take a while, so async makes it so other users can do other operations at the same time
        {
            // If listing is missing some attributes, reload the categories
            if (!ModelState.IsValid)
            {
                ViewBag.Categories =  context.Categories.OrderBy(
                    c=>c.Name).ToList();
                return View(listing);
            }

            if (listing.ImageFile != null && listing.ImageFile.Length>0)
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, "images", "listings");
                Directory.CreateDirectory(uploadsFolder); //If the folder doesn't exist, it makes one
                
                var extension = Path.GetExtension(listing.ImageFile.FileName); //gets the extension of the file name
                var fileName = $"{Guid.NewGuid()}{extension}"; //creates a unique id for the file name

                var filePath = Path.Combine(uploadsFolder, fileName); //creates the full absolute path, combining the listing image path and the file image path

                using (var stream = new FileStream(filePath, FileMode.Create)) //copies the file to the disk
                {
                    await listing.ImageFile.CopyToAsync(stream);
                }

                listing.ImagePath = $"/images/listings/{fileName}"; //relative path, the path stored in the database
            }

            context.Listings.Add(listing);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}

