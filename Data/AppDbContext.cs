using Microsoft.EntityFrameworkCore;
using UniThrift.Models;

namespace UniThrift.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                    Id = 1, Title = "Discrete Math Textbook",
                    Description = "Good condition, highlights in first 2 chapters",
                    Price = 25, CategoryId = "BOOK",
                    Campus = "Fairleigh Dickinson University", IsActive = true,
                    CreatedAt = new DateTime(2025, 11, 12, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}