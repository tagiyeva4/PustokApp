using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.Helpers;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BookController : Controller
    {
        private readonly PustokAppDbContext _context;

        public BookController(PustokAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1,int take=4)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages);
            return View(PaginatedList<Book>.Create(query,take,page));
        }
    }
}
