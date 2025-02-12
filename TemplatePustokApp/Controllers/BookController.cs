using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokAppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookController(PustokAppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int? id)
        {
            return View();
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
           
            var user =await _userManager.GetUserAsync(User);
            if (user != null)
            {
				var vm = getBookDetailVm((int)id, user.Id);

				if (vm.Book is null)
				{
					return NotFound();
				}
				return View(vm);
			}
            else
            {
				var vm = getBookDetailVm((int)id);

				if (vm.Book is null)
				{
					return NotFound();
				}
				return View(vm);
			}

        }
        public async Task<IActionResult> AddComment(BookComment bookComment)
        {
            
            if (!_context.Books.Any(b => b.Id == bookComment.BookId))
            {
                return RedirectToAction("notfound", "error");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null||!await _userManager.IsInRoleAsync(user, "member"))
            {
                return RedirectToAction("Login", "Account", Url.Action("Detail","Book",bookComment.BookId) );
            }
            var vm = getBookDetailVm(bookComment.BookId, user.Id);
            vm.BookComment= bookComment;

            if (!ModelState.IsValid) return View("Detail",vm);

            bookComment.AppUserId = user.Id;
            bookComment.CreatedDate = DateTime.Now;

            _context.BookComments.Add(bookComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", new {id=bookComment.BookId});

        }

        private BookDetailVm getBookDetailVm(int? bookId,string userId)
        {
            var existBook = _context.Books
              .Include(b => b.Author)
              .Include(b => b.BookTags)
              .ThenInclude(bt => bt.Tag)
              .Include(b => b.Genre)
              .Include(b => b.BookImages)
              .Include(b => b.BookComments.Take(2))
              .ThenInclude(bc => bc.AppUser)
              .FirstOrDefault(x => x.Id == bookId);
            //if (existBook == null)
            //{
            //    return NotFound();
            //}

            BookDetailVm vm = new BookDetailVm()
            {
                Book = existBook,
                RelatedBooks = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Where(x => x.Genre.Id == existBook.Genre.Id && x.Id != existBook.Id)
                .Take(5)
                .ToList(),
                HasCommentUser = _context.BookComments.Any(x => x.BookId == existBook.Id && x.AppUserId == userId&& x.Status != CommentStatus.Rejected),

            };
            vm.TotalCommentsCount = existBook.BookComments.Count(x => x.Id != existBook.Id);
            vm.AvarageRate = vm.TotalCommentsCount > 0 ? (int)_context.BookComments.Where(x => x.BookId == existBook.Id).Average(x => x.Rate) : 0;
            return vm;
        }

		private BookDetailVm getBookDetailVm(int? bookId)
		{
			var existBook = _context.Books
			  .Include(b => b.Author)
			  .Include(b => b.BookTags)
			  .ThenInclude(bt => bt.Tag)
			  .Include(b => b.Genre)
			  .Include(b => b.BookImages)
			  .Include(b => b.BookComments.Take(2))
			  .ThenInclude(bc => bc.AppUser)
			  .FirstOrDefault(x => x.Id == bookId);
			//if (existBook == null)
			//{
			//    return NotFound();
			//}

			BookDetailVm vm = new BookDetailVm()
			{
				Book = existBook,
				RelatedBooks = _context.Books
				.Include(b => b.Author)
				.Include(b => b.Genre)
				.Include(b => b.BookImages)
				.Where(x => x.Genre.Id == existBook.Genre.Id && x.Id != existBook.Id)
				.Take(5)
				.ToList(),
				HasCommentUser = _context.BookComments.Any(x => x.BookId == existBook.Id  && x.Status != CommentStatus.Rejected),

			};
			vm.TotalCommentsCount = existBook.BookComments.Count(x => x.Id != existBook.Id);
			vm.AvarageRate = vm.TotalCommentsCount > 0 ? (int)_context.BookComments.Where(x => x.BookId == existBook.Id).Average(x => x.Rate) : 0;
			return vm;
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
