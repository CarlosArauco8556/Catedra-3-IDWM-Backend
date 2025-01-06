namespace Catedra3IDWMBackend.src.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = new User();
    }
}