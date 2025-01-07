namespace Catedra3IDWMBackend.src.DTOs
{
    public class GetPostDto
    {
        public string Title { get; set; } = null!;

        public DateTime PublicationDate { get; set; }

        public string ImageUrl { get; set; } = null!;
    }
}