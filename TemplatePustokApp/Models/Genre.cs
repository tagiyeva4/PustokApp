using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.Models
{
    public class Genre:BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
