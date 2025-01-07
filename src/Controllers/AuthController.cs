using Catedra3IDWMBackend.src.DTOs;
using Catedra3IDWMBackend.src.Interfaces;
using Catedra3IDWMBackend.src.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catedra3IDWMBackend.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (await _userManager.Users.AnyAsync(p => p.Email == registerDto.Email)) return BadRequest( new { message = "El correo ya existe"});

                if (string.IsNullOrEmpty(registerDto.Password)) return BadRequest( new { message = "La contraseña es requerida"});

                var user = new User
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                };
                
                var createUser = await _userManager.CreateAsync(user, registerDto.Password);

                if (createUser.Succeeded)
                {
                    return Ok(new { user = user.Email!});
                }
                else{
                    return StatusCode(500, new { message = createUser.Errors});
                }
            } 
            catch (Exception e)
            {
                return StatusCode(500,  new { message = e.Message});
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return BadRequest(new { message = "Sesion activa" });
                }

                if(!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if(user == null) return Unauthorized( new { message = "Credenciales inválidas"});

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if(!result.Succeeded) return Unauthorized( new { message = "Credenciales inválidas"});

                await _signInManager.SignInAsync(user, isPersistent: true);

                var token = _tokenService.CreateToken(user);

                if (string.IsNullOrEmpty(token)) return Unauthorized( new { message = "Token invalido"});

                return Ok(
                    new NewUserDto
                    {
                        UserName = user.UserName!,
                        Email = user.Email!,
                        Token = token
                    }
                );
            }catch (Exception ex) {
                return StatusCode(500,  new { message = ex.Message});
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return BadRequest(new { message = "No se encontró ninguna sesión activa" });
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                await _tokenService.AddToBlacklistAsync(token);
                await _signInManager.SignOutAsync();
            }

            return Ok(new { message = "Cierre de sesión exitoso" });
        }
    }
}