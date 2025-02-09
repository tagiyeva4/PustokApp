using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Services
{
    public class LayoutServices
    {
        private readonly PustokAppDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly HttpContext httpContext;
        public LayoutServices(PustokAppDbContext context, IHttpContextAccessor httpcontextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
            httpContext = _httpcontextAccessor.HttpContext;

        }

        public List<Genre> GetAllGenres()
        {
            return _context.Genres.ToList();
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(s => s.Key, s => s.Value);
        }

        public List<BasketItemVm> GetUserBasketItems()
        {
            List<BasketItemVm> list;

            var basket = httpContext.Request.Cookies["basket"];
            if (basket != null)
            {
                list = JsonSerializer.Deserialize<List<BasketItemVm>>(basket);
            }
            else
            {
                list = new();
            }

            var user = _context.Users
                .Include(u => u.BasketItems)
                .ThenInclude(bi => bi.Book)
                .FirstOrDefault(u => u.UserName == httpContext.User.Identity.Name);

            if (user != null)
            {
                foreach (var dbBasketItem in user.BasketItems)
                {
                    if (!list.Any(b => b.Id == dbBasketItem.BookId))
                    {
                        BasketItemVm basketItemVm = new BasketItemVm();
                        basketItemVm.Id = dbBasketItem.BookId;
                        basketItemVm.Name = dbBasketItem.Book.Name;
                        basketItemVm.MainImage = dbBasketItem.Book.BookImages.FirstOrDefault(bi => bi.Status == true).Name;
                        if (dbBasketItem.Book.DiscountPercentege > 0)
                        {
                            basketItemVm.Price = dbBasketItem.Book.CostPrice - ((dbBasketItem.Book.CostPrice * dbBasketItem.Book.DiscountPercentege) / 100);
                        }
                        else
                        {
                            basketItemVm.Price = dbBasketItem.Book.CostPrice;
                        }
                        basketItemVm.Count = dbBasketItem.Count;
                        list.Add(basketItemVm);
                    }
                }
            }
            foreach (var item in list)
            {
                var existBook = _context.Books.Include(b => b.BookImages).FirstOrDefault(b => b.Id == item.Id);
                item.Name = existBook.Name;
                item.MainImage = existBook.BookImages.FirstOrDefault(bi => bi.Status == true).Name;
                item.Price = existBook.SalePrice;
            }


            httpContext.Response.Cookies.Append("basket", JsonSerializer.Serialize(list));

            return list;
        }
    }
}
