using Data.Models.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace ConversorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AdminController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;

        }

        [HttpPost("create-admin")]
        [AllowAnonymous] // Permitir crear el primer admin sin token
        public IActionResult CreateAdmin([FromBody] AdminUserDTO adminUserDTO)
        {
            // Verificar si ya existe un admin
            var existingAdmins = _userService.GetAllAdmins();
            if (existingAdmins.Any())
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var admin = _userService.GetUserIdFromToken(token);

                if (!_userService.IsAdmin(admin))
                {
                    return Unauthorized("No tienes permisos para crear un administrador.");
                }
            }

            try
            {
                var adminUser = _userService.CreateAdminUser(adminUserDTO);
                return Ok(new
                {
                    adminUser.UserId,
                    adminUser.Username,
                    adminUser.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete("deactivate/{userId}")]
        public IActionResult DeactivateUser(int userId)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

          
            var admin = _userService.GetUserIdFromToken(token);

            if (!_userService.IsAdmin(admin))
            {
                return Unauthorized("No tienes permisos para crear un administrador.");
            }
            try
            {
                _userService.DeactivateUser(userId, admin);
                return Ok("Usuario desactivado correctamente.");
            }

            catch (ArgumentException)
            {
                return BadRequest("faltan datos/no son correctos");
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateUser([FromBody] UpdateAdminDTO updatedUser)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Obtener el userIdSUB y username
            var admin = _userService.GetUserIdFromToken(token);

            if (!_userService.IsAdmin(admin))
            {
                return Unauthorized("No tienes permisos para crear un administrador.");
            }

            try
            {
                var user = _userService.UpdateUserAdmin(updatedUser, admin);
                return Ok(user);
            }

            catch (ArgumentException)
            {
                return BadRequest("faltan datos/no son correctos");
            }
        }

        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserRegistrationDTO newUser)
        {

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var admin = _userService.GetUserIdFromToken(token);

            if (!_userService.IsAdmin(admin))
            {
                return Unauthorized("No tienes permisos para crear un administrador.");
            }

            try
            {
                var createdUser = _userService.CreateUser(newUser);
                return Ok(createdUser);
            }
            catch (ArgumentException)
            {
                return BadRequest("faltan datos/no son correctos");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAdminById(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var admin = _userService.GetUserIdFromToken(token);
            if (!_userService.IsAdmin(admin))
            {
                return NotFound("Administrador no encontrado.");
            }

            return Ok(admin);
        }

        [Authorize]
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var adminId = _userService.GetUserIdFromToken(token);

                if (!_userService.IsAdmin(adminId))
                {
                    return Unauthorized("No tienes permisos para ver la lista de usuarios.");
                }

                var users = _userService.GetUsers();

                // ✅ Mapeo de User a UserAdminDTO para el frontend
                var userAdmins = users.Select(u => new
                {
                    Id = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    SubscriptionId = u.SubscriptionId,
                    IsActive = u.conversionEnabled // O el campo que indique si está activo
                }).ToList();

                return Ok(userAdmins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }
    }
}
