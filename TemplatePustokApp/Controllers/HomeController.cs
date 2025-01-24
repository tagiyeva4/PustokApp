using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly PustokAppDbContext _context;

        public HomeController(PustokAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVm vm = new HomeVm();

            vm.Sliders=_context.Sliders.ToList();

            vm.Features=_context.Features.ToList();

            vm.NewBooks=_context.Books
                .Include(b=>b.Author)
                .Include(b=>b.BookImages.Where(x=>x.Status!=null))
                .Where(x=>x.IsNew)
                .ToList();

            vm.FeaturedBooks = _context.Books
                .Include(b=>b.Author)
               .Include(b => b.BookImages.Where(x => x.Status != null))
               .Where(x => x.IsFeatured)
               .ToList();

            vm.DiscountBooks = _context.Books.
                Include(b => b.Author)
               .Include(b => b.BookImages.Where(x => x.Status != null))
               .Where(x => x.DiscountPercentege>0)
               .ToList();
            return View(vm);
        }
    }
}
