using Catedra3IDWMBackend.src.Models;

namespace Catedra3IDWMBackend.src.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);

        Task AddToBlacklistAsync(string token);

        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}