using System;
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

        //[ForeignKey]
        [Required(ErrorMessage = "Enter a category.")]
        public string CategoryId { get; set; } = "GEN";
        public Category? Category { get; set; }
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a campus."), StringLength(100)]
        public string Campus { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

    }
}


