using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using NuGet.Protocol;
using TemplatePustokApp.Areas.Manage.ViewModels;
using TemplatePustokApp.Models;
using TemplatePustokApp.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MailKit.Security;
using MailKit.Net.Smtp;
using TemplatePustokApp.Services;

namespace TemplatePustokApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly EmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(UserRegisterVm userRegisterVm)
		{
			if (!ModelState.IsValid) return View();
			AppUser user = await _userManager.FindByNameAsync(userRegisterVm.UserName);

			if (user != null)
			{
				ModelState.AddModelError("", "This username is already exist");
				return View();
			}
			user = new()
			{
				UserName = userRegisterVm.UserName,
				Email = userRegisterVm.Email,
				FullName = userRegisterVm.FullName,
			};
			var result = await _userManager.CreateAsync(user, userRegisterVm.Password);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					// if (error.Code=="DuplicateEmail")
					//{
					// ModelState.AddModelError("Email","This email is already exist");
					//}//ayrica columna xetani yazdirmaq
					ModelState.AddModelError("", error.Description);
				}
				return View();
			}
			await _userManager.AddToRoleAsync(user, "member");

			//send email
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var url = Url.Action("VerifyEmail", "Account", new { email = user.Email, token }, Request.Scheme);

			//send email
			using StreamReader reader = new StreamReader("wwwroot/templates/emailconfirm.html");
			var body = reader.ReadToEnd();
			body = body.Replace("{{{url}}}", url);
			body = body.Replace("{{{username}}}", user.UserName);
			_emailService.SendEmail(user.Email, "Verify Email Address for Pustok", body);
			TempData["Succsess"] = "Email successfully sended to" + user.Email;

			return RedirectToAction("Login");
		}
		public async Task<IActionResult> VerifyEmail(string email,string token)
		{
			var user=await _userManager.FindByEmailAsync(email);
			if (user == null||!await _userManager.IsInRoleAsync(user,"member")) return RedirectToAction("notfound", "error");
			await _userManager.ConfirmEmailAsync(user,token);
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
			TempData["Succsess"] = "Email successfully sended to" ;
			if (!ModelState.IsValid) return View();
			AppUser user = await _userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
			if (user == null || !await _userManager.IsInRoleAsync(user,"member"))
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
			var result = await _signInManager.PasswordSignInAsync(user, userLoginVm.Password, userLoginVm.RememberMe, true);
			if (!user.EmailConfirmed)
			{
				ModelState.AddModelError("", "Email is not confirmed..");
				return View();
			}
			if (result.IsLockedOut)
			{
				ModelState.AddModelError("", "Account is locked out..");
				return View();
			}
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Username or email or password is invalid..");
				return View();
			}

			return RedirectToAction("Index", "Home");
		}
		[Authorize(Roles ="member")]
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}
		[Authorize(Roles = "member")]//ancaq user olarsa gele biler,bu action a
		public async Task<IActionResult> Profile(string tab="dashboard")
		{
			ViewBag.Tab = tab;
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToAction("Login");
			}
			UserProfileVm userprofileVm = new UserProfileVm();
			userprofileVm.UserProfileUpdateVm = new()
			{
				UserName = user.UserName,
				Email = user.Email,
				FullName = user.UserName,
			};

			return View(userprofileVm);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "member")]
		public async Task<IActionResult> Profile(UserProfileUpdateVm userProfileUpdateVm, string tab = "accountdetail")
		{
			ViewBag.Tab=tab;
			UserProfileVm userProfileVm = new ();
			userProfileVm.UserProfileUpdateVm = userProfileUpdateVm;

			if (!ModelState.IsValid) return View(userProfileVm);
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToAction("Login");
			}
			user.FullName = userProfileUpdateVm.FullName;
			user.UserName = userProfileUpdateVm.UserName;
			user.Email = userProfileUpdateVm.Email;

			if(userProfileUpdateVm.NewPassword!=null)
			{
				if(userProfileUpdateVm.CurrentPasword == null)
				{
					ModelState.AddModelError("CurrentPasword", "CurrentPasword is required");
					return View(userProfileVm);
				}
			    var response= await _userManager.ChangePasswordAsync(user,userProfileUpdateVm.CurrentPasword,userProfileUpdateVm.NewPassword);
				if (!response.Succeeded)
				{
					foreach (var error in response.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View(userProfileVm);
				}
			}
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(userProfileVm);
			}
			await _signInManager.SignInAsync(user, true);
			return RedirectToAction("Index","Home");
		}
		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
		{
			if(!ModelState.IsValid) return View();
			var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);
			if (user == null || !await _userManager.IsInRoleAsync(user, "member")) return RedirectToAction("notfound","error");
			var token=await _userManager.GeneratePasswordResetTokenAsync(user);
			var url = Url.Action("ResetPassword", "Account", new {email=user.Email, token },Request.Scheme);

			//send email
            using StreamReader reader = new StreamReader("wwwroot/templates/resetpassword.html");
            var body = reader.ReadToEnd();
            body = body.Replace("{{{url}}}", url);
            body = body.Replace("{{{username}}}", user.UserName);
			_emailService.SendEmail(user.Email, "Reset Password for Login",body);

			#region create email message
			//         // create email message
			//         var email = new MimeMessage();
			//         email.From.Add(MailboxAddress.Parse("tagizadeaysu002@gmail.com"));
			//         email.To.Add(MailboxAddress.Parse(user.Email));
			//         email.Subject = "Verify Email Address for Pustok";
			//         using StreamReader reader = new StreamReader("wwwroot/templates/resetpassword.html");
			//         var body = reader.ReadToEnd();
			//         body = body.Replace("{{{url}}}", url);
			//body = body.Replace("{{{username}}}",user.UserName);
			//email.Body=new TextPart(TextFormat.Html) { Text = body };

			//         // send email
			//         using var smtp = new SmtpClient();
			//         smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
			//         smtp.Authenticate("tagizadeaysu002@gmail.com", "dbsm haxg wsjn mrgv");
			//         smtp.Send(email);
			//         smtp.Disconnect(true);
			#endregion

			TempData["Succsess"] = "Email successfully sended to"+ user.Email;
            return View();
		}
		public IActionResult ResetPassword()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(PasswordResetVm passwordResetVm)
		{
			if (!ModelState.IsValid) return View();
			var user = await _userManager.FindByEmailAsync(passwordResetVm.Email);

			if (user == null || !await _userManager.IsInRoleAsync(user, "member")) return View();
			
			var result=await _userManager.ResetPasswordAsync(user,passwordResetVm.Token,passwordResetVm.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(" ", error.Description);
				}
				return View();
			}
			return RedirectToAction("Login");
		}
	}
}
