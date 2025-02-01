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

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("state", user.IsAdmin.ToString()));
            claimsForToken.Add(new Claim("check", user.conversionEnabled.ToString()));
            claimsForToken.Add(new Claim("Name", user.Username));

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

      
        [Authorize]
        [HttpGet("user/details")]
        public IActionResult GetUserDetails()
        {
            // Extraer el token del encabezado
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Obtener el userIdSUB y username
            var userId = _userService.GetUserIdFromToken(token);

            var user = _userService.GetById(userId);

            if (userId == 0 & user == null)
            {
                return BadRequest("No se pudo extraer informacion del token");
            }
            // Retornar los detalles del usuario
            var userDetailsDto = new UserSubsDetails
            {
                Username = user.Username,
                SubscriptionId = user.SubscriptionId
            };

            return Ok(userDetailsDto);
        }
    }

}
