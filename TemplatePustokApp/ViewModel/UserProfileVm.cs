using TemplatePustokApp.Models;

namespace TemplatePustokApp.ViewModel
{
    public class UserProfileVm
    {
        public UserProfileUpdateVm UserProfileUpdateVm { get; set; }
        public List<Order> Orders { get; set; }
    }
}
