using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;

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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//ancaq bu htmlden sorgu gele biler,postman ve ya basqa yerden sorgu atmaq olmur
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }
            if (_context.Genres.Any(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", "This genre already exist");//xeta yalniz backendden qayidir
                return View();
            }
            genre.CreatedDate = DateTime.Now;
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }
            Genre genre=_context.Genres.Include(g=>g.Books).FirstOrDefault(g=> g.Id == id);
            if (genre == null) {
            return NotFound();
            }
            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if(_context.Genres.Any(g => g.Name.Trim().ToLower() == genre.Name.Trim().ToLower()&&g.Id!=genre.Id))
            {
                ModelState.AddModelError("Name", "This genre already exist");//xeta yalniz backendden qayidir
                return View();
            }
            Genre existGenre = _context.Genres.Include(g => g.Books).FirstOrDefault(g => g.Id == genre.Id);
            if (genre == null)
            {
                return NotFound();
            }
            existGenre.Name = genre.Name;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Genre genre = _context.Genres.Include(g => g.Books).FirstOrDefault(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Genre genre = _context.Genres.Include(g => g.Books).FirstOrDefault(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }
    }
}
