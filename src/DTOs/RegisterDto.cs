using System.ComponentModel.DataAnnotations;

namespace Catedra3IDWMBackend.src.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "El email debe ser una dirección de correo válida.")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica y contener al menos un número.")]
        public string Password { get; set; } = null!;
    }
}