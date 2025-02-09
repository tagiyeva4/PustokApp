using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TemplatePustokApp.Attributes;

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
        [MaxSize(2*1024*1024)]
        [AllowedTypeAttribute("image/jpeg", "image/png")]
        public IFormFile[] Photos{ get; set; }
        //[NotMapped]
        //public IFormFile MainImage { get; set; }
        public List<BookTag> BookTags { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
        public List<BookComment> BookComments { get; set; }
        public Book()
        {
            BookImages = new List<BookImage>();
            BookTags = new List<BookTag>();
        }

    }
}
