using Microsoft.AspNetCore.Mvc;

namespace TemplatePustokApp.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            setItemCookies();
           var result=getItemFromCookies();
            return Content(result);
        }

        public IActionResult setItemCookies()
        {
            //option.Expires=DateTimeOffset.Now.AddSeconds(5);
            var option = new CookieOptions();
            option.MaxAge = TimeSpan.FromSeconds(10);
            Response.Cookies.Append("group", "pb202",option);
            return Content("");
        }

        //public IActionResult getItemFromCookies()
        //{
        //    var group = Request.Cookies["group"];
        //    return View(group); 
        //}

        public string getItemFromCookies()
        {
            var group = Request.Cookies["group"];
            return group;
        }

        //public IActionResult setItemToSession()
        //{
        //    HttpContext.Session.SetString("group","pb202");
        //    return Content("");
        //}
        //public IActionResult getItemFromSession()
        //{
        //    var group=HttpContext.Session.GetString("group");

        //    return Content($"Group: {group}");
        //}
    }
}
