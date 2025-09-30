using Microsoft.AspNetCore.Mvc;

namespace ConversorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        
       [HttpGet]
       public IActionResult Ping()
       {
          return Ok("ConversorAPI is running");
       }
        
    }
}
