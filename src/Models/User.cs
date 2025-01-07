using Microsoft.AspNetCore.Identity;

namespace Catedra3IDWMBackend.src.Models
{
    public class User : IdentityUser
    {
        public new string Email { get; set; } = string.Empty;
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}