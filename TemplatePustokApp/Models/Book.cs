using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemplatePustokApp.Models
{
    public class Book:BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName="decimal(18,2)")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentege { get; set; }
        public bool IsStock { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public int Rate { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<BookImage> BookImages { get; set; }
        [NotMapped]
        public IFormFile[] Photos{ get; set; }
        public Book()
        {
            BookImages = new List<BookImage>();
        }

    }
}
