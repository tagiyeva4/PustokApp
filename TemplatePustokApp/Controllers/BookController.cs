using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;

namespace TemplatePustokApp.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokAppDbContext _context;

        public BookController(PustokAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if (id == null){
                return NotFound();
            }

            var existBook=_context.Books
                .Include(b=>b.Author)
                .Include(b=>b.Genre)
                .Include(b=>b.BookImages)
                .FirstOrDefault(x => x.Id == id);

            if (existBook == null) { 
                return NotFound();
            }

            return View(existBook);
        }
        public IActionResult Modal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existBook = _context.Books
               .Include(b => b.Author)
               .Include(b => b.Genre)
               .Include(b => b.BookImages)
               .FirstOrDefault(x => x.Id == id);

            if (existBook == null)
            {
                return NotFound();
            }

            return PartialView("ModalPartial",existBook);
        }
    }
}
