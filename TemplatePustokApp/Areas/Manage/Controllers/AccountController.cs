using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TemplatePustokApp.Areas.Manage.ViewModels;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> CreateAdminUser()
        {
            AppUser user = new AppUser
            {
                UserName = "_admin",
                Email = "admin@gmail.com"
            };
            IdentityResult identityResult= await _userManager.CreateAsync(user,"_Admin123");
            return Json(identityResult);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm)
        {
            if (!ModelState.IsValid) return View();
            var user=await _userManager.FindByNameAsync(adminLoginVm.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "UserName or Password is incorret...");
                return View();
            }
            var result = await _userManager.CheckPasswordAsync(user, adminLoginVm.Password);
            if (!result)
            {
                ModelState.AddModelError("", "UserName or Password is incorret...");
                return View();
            }
            await _signInManager.SignInAsync(user,false);

            return Content("Succsess");
        }
        public async Task<IActionResult> GetUser()
        {
           var user=await _userManager.GetUserAsync(User);//databaseden hemin useri tapmaq
            return Json(user);
        }
    }
}
