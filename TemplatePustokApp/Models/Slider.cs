using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string Image { get; set; }
        public int Order { get; set; }
    }
}
