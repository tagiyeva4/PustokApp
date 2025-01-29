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
        private readonly IWebHostEnvironment _env;

        public BookController(PustokAppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages);
            return View(PaginatedList<Book>.Create(query, take, page));
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!_context.Genres.Any(g => g.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre not found");
            }
            if (!_context.Authors.Any(a => a.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author not found");
            }

            var files = book.Photos;
            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    if (!file.CheckType(new string[] { "image/jpeg", "image/png" }))
                    {
                        ModelState.AddModelError("Photo", "File type is wrong");
                        return View();
                    }
                    if (file.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Photo", "File legnth must be less than 2 MB");
                        return View();
                    }
                    BookImage bookImage = new BookImage();
                    bookImage.Name = file.SaveImage(_env.WebRootPath, "assets/image/products");
                    if (files[0] == file)
                    {
                        bookImage.Status = true;
                    }
                    book.BookImages.Add(bookImage);
                }
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = _context.Books
                .Include(book => book.BookImages)
                .Include(book => book.Author)
                .Include(book => book.Genre)
                .FirstOrDefault(x => x.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            return View(book);
        }
        public IActionResult DeleteBookImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookImage = _context.BookImages.Find(id);
            if (bookImage is null)
            {
                return NotFound();
            }
            if (bookImage.Status==true)
            {
                return BadRequest();
            }
            _context.BookImages.Remove(bookImage);
            _context.SaveChanges();
            return RedirectToAction("Detail", new { id = bookImage.BookId });
        }

        public IActionResult SetMainImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookImage = _context.BookImages.Find(id);
            if (bookImage is null)
            {
                return NotFound();
            }
            var mainImage = _context.BookImages.FirstOrDefault(bi => bi.Status == true && bi.Id == id);
            if (mainImage is not null)
            {
                mainImage.Status = null;
            }
            bookImage.Status = true;
            _context.SaveChanges();
            return RedirectToAction("Detail", new { id = bookImage.BookId });
        }
    }
}

