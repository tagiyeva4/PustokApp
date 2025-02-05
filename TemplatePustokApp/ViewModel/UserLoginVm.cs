using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.ViewModel
{
	public class UserLoginVm
	{
		[Required]
		public string UserNameOrEmail { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public bool RememberMe { get; set; }	
	}
}
