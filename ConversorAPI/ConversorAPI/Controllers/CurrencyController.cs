using Data.Entities;
using Data.Models.DTOs.CurrencyDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace ConversorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyServices _currencyService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IUserService _userServices;

        public CurrencyController(ICurrencyServices currencyService, ISubscriptionService subscriptionService, IUserService userServices)
        {
            _currencyService = currencyService;
            _subscriptionService = subscriptionService;
            _userServices = userServices;
        }

        
        [HttpGet("currencies")]
        public IActionResult GetAllCurrencies()
        {
            var currencies = _currencyService.GetAllCurrencies();
            if (currencies == null || !currencies.Any())
                return NotFound("No hay monedas disponibles.");

            return Ok(currencies);
        }

        
        [HttpGet("currency/{id}")]
        public IActionResult GetCurrencyById(int id)
        {
            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null)
                return NotFound("Moneda no encontrada o se encuentra en estado BAJA.");

            return Ok(currency);
        }

        
        [HttpPost("currency")]
        public IActionResult AddCurrency([FromBody] CurrencyCreateUpdateDTO currencyDto)
        {
            if (currencyDto == null)
                return BadRequest("Datos inválidos para crear una moneda.");

            var currencyId = _currencyService.AddCurrency(currencyDto);
            return CreatedAtAction(nameof(GetCurrencyById), new { id = currencyId }, currencyDto);
        }

        
        [HttpPut("currency/{id}")]
        public IActionResult UpdateCurrency(int id, [FromBody] CurrencyCreateUpdateDTO currencyDto)
        {
            if (currencyDto == null)
                return BadRequest("Datos inválidos para actualizar la moneda.");

            var result = _currencyService.UpdateCurrency(id, currencyDto);
            if (!result)
                return NotFound("No se pudo actualizar la moneda. Puede que no exista.");

            return NoContent();
        }

        
        [HttpDelete("currency/{id}")]
        public IActionResult DeleteCurrency(int id)
        {
            var result = _currencyService.DeleteCurrency(id);
            if (!result)
                return NotFound("No se pudo eliminar la moneda. Puede que no exista.");

            return NoContent();
        }

        
        [HttpPost("convert")]
        public IActionResult ConvertCurrency([FromBody] ConversionRequestDTO request)
        {
            if (request == null)
                return BadRequest("Datos inválidos para la conversión.");

           
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token no proporcionado.");

            
            var userId = _userServices.GetUserIdFromToken(token);

            if (userId == 0)
                return NotFound("No se pudo extraer el UserId del token.");

            
            var user = _userServices.GetById(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

         
            if (!_userServices.CanConvert(userId))
                return BadRequest("No puedes realizar más conversiones. Actualiza tu suscripción.");

           
            var conversionResult = _currencyService.ConvertCurrency(request);

            if (conversionResult == null)
            {
                return BadRequest("No se pudo realizar la conversión. Verifica los códigos de moneda.");
            }

            
            _userServices.IncrementConversionsUsed(userId);

            
            if (user.ConversionsMaked >= user.Subscription.ConversionLimit)
            {
                user.conversionEnabled = false;
                _userServices.UpdateUser(user);
            }

            return Ok(conversionResult);
        }

    }

}
