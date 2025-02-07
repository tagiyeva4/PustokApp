using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.ViewModel
{
    public class UserProfileUpdateVm
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email{ get; set; }
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string CurrentPasword{ get; set; }
        [DataType(DataType.Password)]
		[MinLength(6)]
		public string NewPassword{ get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword{ get; set; }
}
}
