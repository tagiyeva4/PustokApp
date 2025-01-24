using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class GenreController : Controller
    {
        private readonly PustokAppDbContext _context;

        public GenreController(PustokAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            return View(_context.Genres.Skip((page-1)*2).Take(2).Include(g=>g.Books).ToList());
        }
    }
}
