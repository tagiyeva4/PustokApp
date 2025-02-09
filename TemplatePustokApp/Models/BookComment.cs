namespace TemplatePustokApp.Models
{
	public class BookComment:BaseEntity
	{
		public string Text { get; set; }
		public int Rate { get; set; }
        public string AppUserId{ get; set; }
		public AppUser AppUser { get; set; }
		public int BookId { get; set; }
		public Book Book { get; set; }
		public CommentStatus Status { get; set; } = CommentStatus.Pending;
    }
	public enum CommentStatus
	{
		Pending,
		Accepted,
		Rejected
	}
}
