using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}
