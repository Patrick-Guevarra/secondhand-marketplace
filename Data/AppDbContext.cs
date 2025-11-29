using Microsoft.EntityFrameworkCore;
using UniThrift.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace UniThrift.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = "GEN", Name = "General" },
                new Category { CategoryId = "BOOK", Name = "Textbooks" },
                new Category { CategoryId = "CLO", Name = "Clothing" },
                new Category { CategoryId = "ELEC", Name = "Electronics" }
            );

            //Example listing
            modelBuilder.Entity<Listing>().HasData(
                new Listing
                {
                    Id = 1, Title = "Desk Lamp",
                    Description = "Perfect condition, works just as new",
                    Price = 25, CategoryId = "GEN",
                    Campus = "Fairleigh Dickinson University",
                    ImagePath = "/images/listings/lamp.jpg",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 11, 12, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}