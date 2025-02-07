using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.ViewModel
{
	public class ForgotPasswordVm
	{
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
