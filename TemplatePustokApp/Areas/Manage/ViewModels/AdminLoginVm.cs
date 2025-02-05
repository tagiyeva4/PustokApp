using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.Areas.Manage.ViewModels
{
    public class AdminLoginVm
    {
        [Required(ErrorMessage ="UserName is required")]
        public string UserName { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(10)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
