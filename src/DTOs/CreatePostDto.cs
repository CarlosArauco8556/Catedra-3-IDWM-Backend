using System.ComponentModel.DataAnnotations;

namespace Catedra3IDWMBackend.src.DTOs
{
    public class CreatePostDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "El titulo del post debe tener al menos 5 caracteres.")]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime PublicationDate { get; set; } = DateTime.Now;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}