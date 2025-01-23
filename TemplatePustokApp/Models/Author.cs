using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.Models
{
    public class Author:BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
