using Microsoft.AspNetCore.Identity;

namespace TemplatePustokApp.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName {  get; set; }
    }
}
