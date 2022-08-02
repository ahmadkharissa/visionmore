using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Visionmore.Data {
    public class Product {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product")]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Image { get; set; }

        public string Color { get; set; }

        public bool Available { get; set; }
    }
}
