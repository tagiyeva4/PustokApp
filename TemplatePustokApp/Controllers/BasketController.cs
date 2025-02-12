using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Controllers
{
    public class BasketController : Controller
    {
       private readonly PustokAppDbContext _pustokAppDbContext;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(PustokAppDbContext pustokAppDbContext, UserManager<AppUser> userManager)
        {
            _pustokAppDbContext = pustokAppDbContext;
            _userManager = userManager;
        }

        public ActionResult Add(int? id)
        {
            if (id == null) return NotFound();
            
            var book =_pustokAppDbContext.Books
                .Include(b=>b.BookImages)
                .FirstOrDefault(b=>b.Id == id);
            if(book == null)return NotFound();
            var basket = HttpContext.Request.Cookies["basket"];
            List<BasketItemVm> basketItemVms ;
            if (basket == null)
            {
                basketItemVms = new ();
            }
            else
            {
                basketItemVms=JsonSerializer.Deserialize<List<BasketItemVm>>(basket);//ozune cevirir
            }
            var basketItemVm=basketItemVms.FirstOrDefault(b=>b.Id==id);
            if (basketItemVm == null)
            {
                BasketItemVm basketItem = new ();
                basketItem.Id=book.Id;
                basketItem.Name=book.Name;
                basketItem.MainImage = book.BookImages.FirstOrDefault(bi=>bi.Status==true).Name;
                if (book.DiscountPercentege > 0) { 
                basketItem.Price = book.CostPrice - ((book.CostPrice * book.DiscountPercentege) / 100);
                }
                else
                {
                    basketItem.Price = book.CostPrice;
                }
                basketItem.Count = 1;
                basketItemVms.Add(basketItem);
            }
            else
            {
                basketItemVm.Count++;
            }
            if(User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                var user=_userManager.Users.Include(u=>u.BasketItems).FirstOrDefault(u=>u.UserName==User.Identity.Name);
                var basketItem=user.BasketItems.FirstOrDefault(b=>b.BookId==id);
                if (basketItem==null)
                {
                    BasketItem newBasketItem = new();
                    newBasketItem.BookId=book.Id;
                    newBasketItem.Count=1;
                    newBasketItem.AppUserId=user.Id;
                    newBasketItem.CreatedDate=DateTime.Now;
                    user.BasketItems.Add(newBasketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                _pustokAppDbContext.SaveChanges();
            }
            

            HttpContext.Response.Cookies.Append("basket",JsonSerializer.Serialize(basketItemVms));//string e cevirir  ,js de jsonstringfy jsonparse  

            return PartialView("_BasketPartial",basketItemVms);
        }


        public IActionResult Index()
        {
            var basket = HttpContext.Request.Cookies["basket"];
            List<BasketItemVm> basketItemVms;
            if(basket==null)
            {
                basketItemVms = new();
            }
            else
            {
                basketItemVms=JsonSerializer.Deserialize<List<BasketItemVm>>(basket);
            }
            return View(basketItemVms);
        }

       

        public IActionResult setItemCookies()
        {
            //option.Expires=DateTimeOffset.Now.AddSeconds(5);
            var option = new CookieOptions();
            option.MaxAge = TimeSpan.FromSeconds(10);
            Response.Cookies.Append("group", "pb202",option);
            return Content("");
        }

        public string getItemFromCookies()
        {
            var group = Request.Cookies["group"];
            return group;
        }

        //public IActionResult getItemFromCookies()
        //{
        //    var group = Request.Cookies["group"];
        //    return View(group); 
        //}
        //public IActionResult Index()
        //{
        //    setItemCookies();
        //   var result=getItemFromCookies();
        //    return Content(result);
        //}
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
