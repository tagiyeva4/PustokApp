using System.ComponentModel.DataAnnotations;

namespace TemplatePustokApp.ViewModel
{
    public class UserRegisterVm
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email{ get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password{ get; set; }
        [DataType(DataType.Password)]
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword{ get; set; }
}
}
