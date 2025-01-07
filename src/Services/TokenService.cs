using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Catedra3IDWMBackend.src.Data;
using Catedra3IDWMBackend.src.Interfaces;
using Catedra3IDWMBackend.src.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Catedra3IDWMBackend.src.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDBContext _context;

        public TokenService(UserManager<User> userManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;

            var signingKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? throw new ArgumentNullException("Signing key no puede ser nula o vacía"); 
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!), 
                new Claim(ClaimTypes.NameIdentifier, user.Id), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentNullException("JWT Issuer no puede ser nula o vacia");;
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new ArgumentNullException("JWT Audience no puede ser nula o vacia");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), 
                Expires = DateTime.Now.AddDays(4), 
                SigningCredentials = creds, 
                Issuer = issuer, 
                Audience = audience 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task AddToBlacklistAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiration = jwtToken.ValidTo;

            if (jti != null)
            {
                var blacklistedToken = new BlacklistedToken
                {
                    TokenId = jti,
                    Expiration = expiration
                };

                _context.BlacklistedTokens.Add(blacklistedToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti != null)
            {
                var blacklistedToken = await _context.BlacklistedTokens
                    .FirstOrDefaultAsync(bt => bt.TokenId == jti);

                if (blacklistedToken != null && blacklistedToken.Expiration > DateTime.UtcNow)
                {
                    return true;
                }
            }

            return false;
        }
    }
}