using System.ComponentModel.DataAnnotations;

namespace UniThrift.Models
{
    public class Category{
        [Key]
        [Required, StringLength(8)]
        public string CategoryId { get; set; } = "GEN";

        [Required, StringLength(40)]
        public string Name { get; set; } = "General";
    }
}


