using Data.Models.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConversorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public UserController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        // Autenticación de usuarios y generación de JWT
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] UserCredentials userCredentials)
        {
            var user = _userService.ValidateUser(userCredentials.Username, userCredentials.Password);

            if (user is null)
                return Unauthorized("Credenciales inválidas.");

            // Crear el token JWT
            var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]));
            var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
        {
            new Claim("sub", user.UserId.ToString()),
            new Claim("state", user.SubCount.ToString()),
            new Claim("username", user.Username)
        };

            var jwtSecurityToken = new JwtSecurityToken(
                _config["Authentication:Issuer"],
                _config["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                credentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(new { token = tokenToReturn });
        }

        // Registro de usuarios
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistrationDTO registrationDto)
        {
            try
            {
                var user = _userService.CreateUser(registrationDto);
                return Ok($"Hola {registrationDto.Username}, ¡bienvenido a Currency APP!");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Obtener detalles del usuario autenticado
        [Authorize]
        [HttpGet("usuarios/detalles")]
        public IActionResult GetUserDetails()
        {
            // Extraer el token del encabezado
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token no proporcionado.");

            // Obtener el UserId del token
            var userId = _userService.GetUserIdFromToken(token);
            if (userId == 0)
                return BadRequest("No se pudo extraer información del token.");

            var user = _userService.GetById(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            // Crear y retornar los detalles del usuario
            var userDetailsDto = new UserSubsDetails
            {
                Username = user.Username,
                SubscriptionId = user.SubscriptionId
            };

            return Ok(userDetailsDto);
        }
    }

}
