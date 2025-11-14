using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniThrift.Models
{
    public class Listing
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter a title."), StringLength(80)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a price."), Range(0, 10000)]
        public decimal Price { get; set; }

        // ForeignKey
        [Required(ErrorMessage = "Enter a category.")]
        public string CategoryId { get; set; } = "GEN";
        public Category? Category { get; set; } //Category? because its only used during POST, so its null when loading the listing from db
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a campus."), StringLength(100)]
        public string Campus { get; set; } = string.Empty;

        // Image Handling
        [Required(ErrorMessage = "Upload a photo.")]
        public string ImagePath { get; set; } = string.Empty;
        [Required(ErrorMessage = "Upload a photo."), NotMapped] // Doesnt map ImageFile to a column, only makes a column for the ImagePath
        public IFormFile? ImageFile { get; set; } //IFormFile? because its only used during POST, so its null when loading the listing from db

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

    }
}


