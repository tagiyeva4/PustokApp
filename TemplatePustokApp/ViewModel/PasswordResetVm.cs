using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.ViewModel
{
	public class PasswordResetVm
	{
		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		[Required]
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }
		public string Email { get; set; }
		public string Token { get; set; }
	}
}
