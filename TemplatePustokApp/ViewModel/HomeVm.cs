using TemplatePustokApp.Models;

namespace TemplatePustokApp.ViewModel
{
    public class HomeVm
    {
        public List<Feature> Features { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> DiscountBooks { get; set; }
    }
}
