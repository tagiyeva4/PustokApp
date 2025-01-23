namespace TemplatePustokApp.Models
{
    public class BookImage:BaseEntity
    {
        public string Name { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public bool? Status { get; set; } //3 cur deyer veririk true false null.{t ,f ozu ve hover} 
    }
}
