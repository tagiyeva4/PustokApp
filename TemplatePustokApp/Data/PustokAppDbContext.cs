using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Data
{
	public class PustokAppDbContext : IdentityDbContext<AppUser>
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
		public DbSet<Setting> Settings { get; set; }
		public DbSet<BookTag> BookTags { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<AppUser> AppUsers { get; set; }
		public DbSet<BookComment> BookComments { get; set; }
		public DbSet<BasketItem> BasketItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public override int SaveChanges()
		{
			var entries = ChangeTracker.Entries<BaseEntity>();
			foreach (var entire in entries)
			{
				if (entire.State == EntityState.Added)
				{
					entire.Property(p => p.CreatedDate).CurrentValue = DateTime.Now;
				}
				if (entire.State == EntityState.Modified)
				{
					entire.Property(p => p.UpdatedDate).CurrentValue = DateTime.Now;
				}
				//if (entire.Property(p => p.Isdelete.CurrentValue == true))
				// {
				//entire.Property(p => p.DeletedDate).CurrentValue = DateTime.Now;
				//}
				//if (entire.State == EntityState.Deleted)
				//{
				// throw new Exception("You cannot delete..");
				//}soft delete yazanda
			}
			return base.SaveChanges();
		}
		//save change virtual metoddur ,override edilir
	}


}
