using System.ComponentModel.DataAnnotations;

namespace Catedra3IDWMBackend.src.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica y contener al menos un número.")]
        public string Password { get; set; } = null!;
    }
}