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

    
        [HttpPost("assign")]
        public IActionResult AssignSubscriptionToUser([FromBody] UserSubscriptionDTO userSubscriptionDto)
        {
            var status = _subscriptionService.MakeSubscriptionToUser(userSubscriptionDto);

            if (status == null)
            {
                return BadRequest("No se pudo asignar la suscripción. Verifique los datos.");
            }

            return Ok(new
            {
                Message = $"La suscripción {status.SubscriptionName} fue asignada exitosamente al usuario {status.UserId}.",
                Status = status
            });
        }

     
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
