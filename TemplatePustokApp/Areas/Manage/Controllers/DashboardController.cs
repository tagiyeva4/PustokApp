using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
	[Authorize(Roles = "admin,superadmin")]
	public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
