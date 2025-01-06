namespace Catedra3IDWMBackend.src.Models
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string TokenId { get; set; } = string.Empty;
        public DateTime Expiration { get; set; } 
    }
}