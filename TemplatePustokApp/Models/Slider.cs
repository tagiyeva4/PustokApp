using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemplatePustokApp.Models
{
    public class Slider:BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
        [Required]
        [MaxLength(100)]
        public string ButtonText { get; set; }
        [Required]
        [MaxLength(100)]
        public string ButtonLink { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        [NotMapped]//Sql-den ignore ucun
        public IFormFile Photo{ get; set; }
    }
}
