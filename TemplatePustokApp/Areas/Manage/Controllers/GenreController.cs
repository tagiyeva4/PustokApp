using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.Helpers;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]
    public class GenreController : Controller
    {
        private readonly PustokAppDbContext _context;

        public GenreController(PustokAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1,int take=2)
        {
           var query=_context.Genres.Include(g=>g.Books);//icra olunmamis query
            #region
            //var count = query.Count();
            //var data=query.Skip((page - 1) * take).Take(take).Include(g => g.Books).ToList();
            //ViewBag.pageCount =Math.Ceiling((decimal)count / take);//yuxariya yuvarlaqlasdirir,decimal ile isleyir
            //ViewBag.currentPage=page;
            //ViewBag.hasNext = page < ViewBag.pageCount;
            //ViewBag.hasPrevious = page > 1;
            #endregion
            PaginatedList<Genre>paginated=PaginatedList<Genre>.Create(query, take,page);
            return View(paginated);
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

            return Ok();
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
