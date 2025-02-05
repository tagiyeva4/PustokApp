using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TemplatePustokApp.Areas.Manage.ViewModels;
using TemplatePustokApp.Models;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterVm userregistervm)
        {
            if(!ModelState.IsValid) return View();
            AppUser user= await _userManager.FindByNameAsync(userregistervm.UserName);

            if(user != null)
            {
                ModelState.AddModelError("UserName","This username is already exist");
                return View();
            }
            user = new()
            {
                UserName = userregistervm.UserName,
                Email = userregistervm.Email,
                FullName = userregistervm.FullName,
            };
            var result=await _userManager.CreateAsync(user,userregistervm.Password);
           
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code=="DuplicateEmail")
                    {
                        ModelState.AddModelError("Email","This email is already exist");
					}//ayrica columna xetani yazdirmaq
                    ModelState.AddModelError("",error.Description);  
                }
				return View();
			}

            return RedirectToAction("Login");
        }
        public IActionResult Login() 
        {
            return View(); 
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginVm userLoginVm)
        {
            if(!ModelState.IsValid) return View();
            AppUser user=await _userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(userLoginVm.UserNameOrEmail);
                if (user == null)
                {
					ModelState.AddModelError("", "Username or email is invalid..");
                    return View();
				}
            }
            #region
            //var result=await _userManager.CheckPasswordAsync(user, userLoginVm.Password);
            //if (!result)
            //{
            //	ModelState.AddModelError("", "UserName or Password is incorret...");
            //	return View();
            //}
            //await _signInManager.SignInAsync(user, false);
            #endregion
            var result = await _signInManager.PasswordSignInAsync(user, userLoginVm.Password,userLoginVm.RememberMe,true);
            if (result.IsLockedOut)
            {
				ModelState.AddModelError("", "Account is locked out..");
				return View();
			}
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or email or password is invalid..");
                return View();
            }

			return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> LogOut()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult Profile()
        {
            return View();
        }

	}
}
