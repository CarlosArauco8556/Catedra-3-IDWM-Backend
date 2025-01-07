using System.ComponentModel.DataAnnotations;

namespace Catedra3IDWMBackend.src.DTOs
{
    public class CreatePostDto
    {
        [Required]
        [MinLength(5)]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime PublicationDate { get; set; } = DateTime.Now;

        [Required]
        public IFormFile ImageUrl { get; set; } = null!;
    }
}