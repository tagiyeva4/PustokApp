using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Data;
using TemplatePustokApp.ViewModel;

namespace TemplatePustokApp.Controllers
{
	public class ShopController : Controller
	{
		private readonly PustokAppDbContext _context;

		public ShopController(PustokAppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index(int? genreId = null, List<int>? authorIds = null, List<int>? tagIds = null,string sort="AtoZ",int? minPrice=null,int? maxPrice=null)
		{
			ShopVm shopVm = new ShopVm();
			shopVm.Authors = _context.Authors.Include(a => a.Books).ToList();
			shopVm.Genres = _context.Genres.Include(g => g.Books).ToList();
			shopVm.Tags = _context.Tags.ToList();
			var query = _context.Books
				.Include(b => b.Author)
				.Include(b => b.Genre)
				.Include(b => b.BookImages.Where(bi => bi.Status != null))
				.AsQueryable();
			if(genreId != null)
			{
				query = query.Where(b => b.GenreId == genreId);
			}	
			if(authorIds != null)
			{
				query=query.Where(b=>authorIds.Contains(b.Author.Id));
			}
			if(tagIds != null)
			{
				query=query.Where(b=>b.BookTags.Any(bt=>tagIds.Contains(bt.TagId)));
			}
			if(minPrice != null && maxPrice != null)
			{
				query = query.Where(b => b.DiscountPercentege > 0 ? (b.CostPrice - ((b.CostPrice * b.DiscountPercentege) / 100)>=minPrice && b.CostPrice - ((b.CostPrice * b.DiscountPercentege) / 100)<=maxPrice) : (b.CostPrice>=minPrice && b.CostPrice <= maxPrice));
					
			}
			switch (sort)
			{
				case "ZtoA":
					query = query.OrderByDescending(b => b.Name);
					break;
				case "PriceHighToLow":
					query = query.OrderByDescending(b => b.DiscountPercentege>0? b.CostPrice-((b.CostPrice*b.DiscountPercentege)/100):b.CostPrice);
					break;
				case "PriceLowToHigh":
					query = query.OrderBy(b => b.DiscountPercentege > 0 ? b.CostPrice - ((b.CostPrice * b.DiscountPercentege) / 100) : b.CostPrice);
					break;

				default:
					query=query.OrderBy(b => b.Name);
					break;

			}
			shopVm.Books=query.ToList();
			ViewBag.GenreId = genreId;
			ViewBag.TagIds=tagIds;
			ViewBag.AuthorIds=authorIds;
			ViewBag.Sort=sort;
			ViewBag.SortList = new List<SelectListItem>() 
			{ 
				new SelectListItem(){Text="A to Z",Value="AtoZ",Selected=sort=="AtoZ"},
				new SelectListItem(){Text="Z to A",Value="ZtoA",Selected=sort=="ZtoA"},
				new SelectListItem(){Text="High To Low",Value="PriceHighToLow",Selected=sort=="PriceHighToLow"},
				new SelectListItem(){Text="Low To High",Value="PriceLowToHigh",Selected=sort=="PriceLowToHigh"}
			};
			ViewBag.MinPrice=_context.Books.Min(b=>b.CostPrice);
			ViewBag.MaxPrice=_context.Books.Max(b=>b.CostPrice);
			ViewBag.SelectedMinPrice = minPrice ?? ViewBag.MinPrice;
			ViewBag.SelectedMaxPrice = maxPrice ?? ViewBag.MaxPrice;
			return View(shopVm);
		}
	}
}
