using Microsoft.AspNetCore.Mvc;

namespace BCImplementatie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GebruikController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetGebruikById(Guid id)
        {
            // Placeholder response
            return Ok(new { Id = id, Status = "Actief", ClientId = Guid.NewGuid() });
        }

        [HttpPost("start")]
        public IActionResult StartGebruik()
        {
            // Placeholder POST handler
            return Ok("Gebruik gestart");
        }
    }
}
