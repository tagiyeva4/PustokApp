using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Data
{
    public class PustokAppDbContext : DbContext
    {
        public PustokAppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Slider> Sliders { get; set; } //CRUD emeliyyatlarin verir Dbsetler
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Genre> Genres { get; set; }

    }
}
