using Data.Models.DTOs.SubscriptionDTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace ConversorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // Asignar una suscripción a un usuario
        [HttpPost("assign")]
        public IActionResult AssignSubscriptionToUser([FromBody] UserSubscriptionDTO userSubscriptionDto)
        {
            if (userSubscriptionDto == null || userSubscriptionDto.UserId <= 0 || userSubscriptionDto.SubscriptionId <= 0)
            {
                return BadRequest("Datos inválidos. Asegúrese de proporcionar un UserId y SubscriptionId válidos.");
            }

            var status = _subscriptionService.MakeSubscriptionToUser(userSubscriptionDto);

            if (status == null)
            {
                return BadRequest("No se pudo asignar la suscripción. Verifique que el usuario y la suscripción existan.");
            }

            return Ok(new
            {
                Message = $"La suscripción {status.SubscriptionName} fue asignada exitosamente al usuario {status.UserId}.",
                Status = status
            });
        }

        // Obtener detalles de una suscripción específica
        [HttpGet("{subscriptionId}")]
        public IActionResult GetSubscriptionDetails(int subscriptionId)
        {
            if (subscriptionId <= 0)
            {
                return BadRequest("SubscriptionId debe ser un valor válido.");
            }

            var subscription = _subscriptionService.GetSubscriptionById(subscriptionId);
            if (subscription == null)
            {
                return NotFound($"No se encontró una suscripción con el ID {subscriptionId}.");
            }

            return Ok(subscription);
        }

        // Listar todas las suscripciones disponibles
        [HttpGet("list")]
        public IActionResult GetAllSubscriptions()
        {
            var subscriptions = _subscriptionService.GetAllSubscriptions();
            if (subscriptions == null || !subscriptions.Any())
            {
                return NotFound("No hay suscripciones disponibles.");
            }

            return Ok(subscriptions);
        }
    }

}
