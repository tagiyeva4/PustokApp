using TemplatePustokApp.Models;

namespace TemplatePustokApp.ViewModel
{
    public class ShopVm
    {
        public List<Author> Authors { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Book> Books { get; set; }
    }
}
