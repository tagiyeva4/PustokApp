using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.Helpers;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="admin,superadmin")]
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
            ViewBag.Tags=_context.Tags.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            book.CreatedDate = DateTime.Now;
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
            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(t=>t.Id==tagId))
                {
                    ModelState.AddModelError("TagIds", "There is no tag in this id...");
                    return View();
                }
                BookTag bookTag = new BookTag();
                bookTag.TagId = tagId;
                bookTag.Book = book;
                book.BookTags.Add(bookTag);
            }

            var files = book.Photos;
            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    #region
                    //if (!file.CheckType(new string[] { "image/jpeg", "image/png" }))
                    //{
                    //    ModelState.AddModelError("Photo", "File type is wrong");
                    //    return View();
                    //}
                    //if (file.Length > 2 * 1024 * 1024)
                    //{
                    //    ModelState.AddModelError("Photo", "File legnth must be less than 2 MB");
                    //    return View();
                    //}
                    #endregion
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
                .Include(book => book.BookTags)
                .ThenInclude(bt=>bt.Tag)
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
            return RedirectToAction("Edit", new { id = bookImage.BookId });
        }

        public IActionResult SetMainImage(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            var bookImage = _context.BookImages.Find(id);
            if (bookImage is null)
            {
                return NotFound();
            }
            var mainImage = _context.BookImages.FirstOrDefault(bi => bi.Status == true && bi.BookId == bookImage.BookId);
            mainImage.Status = null;
            bookImage.Status = true;
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = bookImage.BookId });
        }
        public IActionResult Edit(int? id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            if (id == null)
            {
                return NotFound();
            }
            var book = _context.Books
                .Include(book => book.BookImages)
                .Include(book => book.Author)
                .Include(book => book.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(book => book.Genre)
                .FirstOrDefault(x => x.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            book.TagIds=book.BookTags.Select(x => x.TagId).ToList();
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existbook = _context.Books.Find(book.Id);
            if (existbook is null)
            {
                return NotFound();
            }
            if (existbook.GenreId != book.GenreId)
            {
                if (!_context.Genres.Any(g => g.Id == book.GenreId))
                {
                    ModelState.AddModelError("GenreId", "Genre not found");
                }
            }
            if (existbook.AuthorId!=book.AuthorId)
            {
                if (!_context.Authors.Any(a => a.Id == book.AuthorId))
                {
                    ModelState.AddModelError("AuthorId", "Author not found");
                }
            }
            var files = book.Photos;
            if (files!=null)
            {
                foreach (var file in files)
                {
                    BookImage bookImage = new BookImage();
                    bookImage.Name = file.SaveImage(_env.WebRootPath, "assets/image/products");
                   existbook.BookImages.Add(bookImage);
                }
            }
            List<BookTag> bookTags = new List<BookTag>();
            foreach (var tagId in book.TagIds.ToList())
            {
                if (!_context.Tags.Any(t => t.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", "There is no tag in this id...");
                    return View();
                }
                BookTag bookTag = new BookTag();
                bookTag.TagId = tagId;
                bookTag.Book=existbook;
                bookTags.Add(bookTag);
            }
            var existBookTags = _context.BookTags.Where(bt=>bt.BookId==existbook.Id).ToList();
            foreach (var bookTag in existBookTags)
            {
                _context.BookTags.Remove(bookTag);   
            }
            
            existbook.BookTags = bookTags;
            existbook.Name = book.Name;
            existbook.Description = book.Description;
            existbook.AuthorId = book.AuthorId;
            existbook.GenreId = book.GenreId;
            existbook.Photos = book.Photos;
            existbook.IsStock = book.IsStock;
            existbook.IsFeatured = book.IsFeatured;
            existbook.IsNew = book.IsNew;
            existbook.Rate = book.Rate;
            existbook.SalePrice = book.SalePrice;
            existbook.CostPrice = book.CostPrice;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

