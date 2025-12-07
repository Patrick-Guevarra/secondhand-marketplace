using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniThrift.Models;
using UniThrift.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UniThrift.Controllers
{
    [Authorize]
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
        [AllowAnonymous] //Does not require auth
        public IActionResult Index(string? query, string? categoryId, string? campus)
        {
            // Build a queryable feed to layer on filters before pulling data back
            var listingsQuery = context.Listings
                .Include(l => l.Category)
                .Where(l => l.IsActive)
                .AsQueryable();

            // Text search across title and description (case-insensitive match on the database side)
            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim();
                listingsQuery = listingsQuery.Where(l =>
                    EF.Functions.Like(l.Title, $"%{term}%") ||
                    EF.Functions.Like(l.Description, $"%{term}%"));
            }

            // category filter
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                listingsQuery = listingsQuery.Where(l => l.CategoryId == categoryId);
            }

            // Optional campus filter 
            if (!string.IsNullOrWhiteSpace(campus))
            {
                listingsQuery = listingsQuery.Where(l => l.Campus == campus);
            }

            // Order newest first and execute the query
            var listings = listingsQuery
                .OrderByDescending(l => l.CreatedAt)
                .ToList();

            // Send the filters and selections back to the view
            ViewBag.Categories = context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.Query = query;
            ViewBag.CategoryId = categoryId;
            ViewBag.Campus = campus;

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

            // tie listing to current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            listing.UserId = userId;

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
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Listings");
        }

        [Authorize]
        public IActionResult MyListings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var listings = context.Listings
                .Include(l => l.Category)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();

            return View(listings);
        }

        [HttpGet] 
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var listing = await context.Listings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (listing == null)
            {
                return NotFound(); 
            }

            ViewBag.Categories = context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            return View(listing);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Listing listing)
        {
            if (id != listing.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existing = await context.Listings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (existing == null)
            {
                return NotFound(); 
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = context.Categories
                    .OrderBy(c => c.Name)
                    .ToList();
                return View(listing);
            }

            // update basic fields
            existing.Title = listing.Title;
            existing.Description = listing.Description;
            existing.Price = listing.Price;
            existing.CategoryId = listing.CategoryId;
            existing.Campus = listing.Campus;
            existing.IsActive = listing.IsActive;

            // handle optional new image upload
            if (listing.ImageFile != null && listing.ImageFile.Length > 0)
            {
                // delete old image if exists
                if (!string.IsNullOrEmpty(existing.ImagePath))
                {
                    var oldPath = Path.Combine(env.WebRootPath, existing.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var uploadsFolder = Path.Combine(env.WebRootPath, "images", "listings");
                Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(listing.ImageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await listing.ImageFile.CopyToAsync(stream);
                }

                existing.ImagePath = $"/images/listings/{fileName}";
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(MyListings));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var listing = await context.Listings
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (listing == null)
            {
                return NotFound(); 
            }

            return View(listing);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var listing = await context.Listings
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (listing == null)
            {
                return NotFound(); 
            }

            // delete image file
            if (!string.IsNullOrEmpty(listing.ImagePath))
            {
                var fullPath = Path.Combine(env.WebRootPath, listing.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            context.Listings.Remove(listing);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(MyListings));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var listing = await context.Listings
                .Include(l => l.Category)
                .Include(l => l.User) 
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

    }
}
