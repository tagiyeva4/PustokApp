using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Controllers
{
	public class OrderController : Controller
	{
		private readonly PustokAppDbContext _pustokAppDbContext;
		private readonly UserManager<AppUser> _userManager;

		public OrderController(PustokAppDbContext pustokAppDbContext, UserManager<AppUser> userManager)
		{
			_pustokAppDbContext = pustokAppDbContext;
			_userManager = userManager;
		}
        [Authorize(Roles = "member")]
        public IActionResult Cancel(int orderId)
        {
            var order = _pustokAppDbContext.Orders
                .Where(o => o.AppUserId == _userManager.GetUserId(User))
               .FirstOrDefault(o => o.Id == orderId);
            order.OrderStatus = OrderStatus.Cancelled;
            _pustokAppDbContext.SaveChanges();
            return RedirectToAction("Profile","Account",new {tab="orders"});
        }

        [Authorize(Roles = "member")]
        public IActionResult GetOrderItems(int orderId)
		{
            var order = _pustokAppDbContext.Orders
                .Where(o => o.AppUserId==_userManager.GetUserId(User))
                .Include(o => o.OrderItems)
                .ThenInclude(oi=>oi.Book)
               .FirstOrDefault(o => o.Id == orderId);
            return PartialView("_OrderItemsPartial",order);
		}

        [Authorize(Roles = "member")]
        public IActionResult CheckOut()
        {
            var user = _userManager.Users
                .Include(u => u.BasketItems)
                .ThenInclude(bi => bi.Book)
                .FirstOrDefault(u => u.UserName == User.Identity.Name);
            CheckOutVm checkOutVm = new CheckOutVm();
            checkOutVm.CheckoutItemVms = user.BasketItems.Select(b => new CheckoutItemVm
            {
                ProductName = b.Book.Name,
                TotalItemPrice = b.Book.DiscountPercentege > 0 ? (b.Book.CostPrice - (b.Book.CostPrice * b.Book.DiscountPercentege) / 100) * b.Count : b.Book.CostPrice * b.Count,
                Count = b.Count,
            }).ToList();
            return View(checkOutVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public IActionResult CheckOut(OrderVm orderVm)
        {
            var user = _userManager.Users
               .Include(u => u.BasketItems)
               .ThenInclude(bi => bi.Book)
               .FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!ModelState.IsValid)
            {
                CheckOutVm checkOutVm = new CheckOutVm();
                checkOutVm.CheckoutItemVms = user.BasketItems.Select(b => new CheckoutItemVm
                {
                    ProductName = b.Book.Name,
                    TotalItemPrice = b.Book.DiscountPercentege > 0 ? (b.Book.CostPrice - (b.Book.CostPrice * b.Book.DiscountPercentege) / 100) * b.Count : b.Book.CostPrice * b.Count,
                    Count = b.Count,
                }).ToList();
                checkOutVm.OrderVm = orderVm;
                return View(checkOutVm);
            }
            Order order = new Order();
            order.AppUserId = user.Id;
            order.Country = orderVm.Country;
            order.ZipCode = orderVm.ZipCode;
            order.TownCity = orderVm.TownCity;
            order.Address = orderVm.Address;
            order.TotalPrice = user.BasketItems.Sum(b => b.Book.DiscountPercentege > 0 ? (b.Book.CostPrice - (b.Book.CostPrice * b.Book.DiscountPercentege) / 100) * b.Count : b.Book.CostPrice * b.Count);
            order.State = orderVm.State;
            order.OrderStatus = OrderStatus.Pending;
            order.OrderItems = user.BasketItems.Select(b => new OrderItem
            {
                BookId = b.BookId,
                Count = b.Count,
            }).ToList();
            _pustokAppDbContext.Orders.Add(order);
            _pustokAppDbContext.BasketItems.RemoveRange(user.BasketItems);
            _pustokAppDbContext.SaveChanges();
            HttpContext.Response.Cookies.Delete("basket");
            return RedirectToAction("Profile", "Account", new { tab = "orders" });
        }
    }
}
