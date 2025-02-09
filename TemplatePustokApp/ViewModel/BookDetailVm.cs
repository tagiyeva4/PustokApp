using TemplatePustokApp.Models;

namespace TemplatePustokApp.ViewModel
{
	public class BookDetailVm
	{
		public Book Book { get; set; }
		public List<Book> RelatedBooks { get; set; }
		public bool HasCommentUser { get; set; }
		public int TotalCommentsCount { get; set; }
        public int AvarageRate { get; set; }
		public BookComment BookComment { get; set; }
    }
}
