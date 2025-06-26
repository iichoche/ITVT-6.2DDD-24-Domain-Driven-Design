using BCImplementatie.Application.DTOs;
using BCImplementatie.Application.Commands.CareNeeds;
using BCImplementatie.Application.Commands.StartGebruik;
using BCImplementatie.Application.Queries.Gebruiken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BCImplementatie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GebruikenController : ControllerBase
    {
        private readonly IMediator _med;
        public GebruikenController(IMediator med) => _med = med;

        // GET api/gebruiken
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _med.Send(new GetAllGebruikQuery()));

        // GET api/gebruiken/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
            => Ok(await _med.Send(new GetGebruikByIdQuery(id)));

        // POST api/gebruiken
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGebruikDto dto)
        {
            var id = await _med.Send(new StartGebruikCommand(dto.ClientId, dto.ProductItemId));
            return CreatedAtAction(nameof(Get), new { id }, new { gebruikId = id });
        }

        // PUT api/gebruiken/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGebruikDto dto)
        {
            if (!dto.InGebruik)
                await _med.Send(new StopGebruikCommand(id));
            // eventueel andere update-cases...
            return NoContent();
        }

        // DELETE api/gebruiken/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _med.Send(new DeleteGebruikCommand(id));
            return NoContent();
        }

        // DELETE api/gebruiken?ids={id1},{id2}
        [HttpDelete]
        public async Task<IActionResult> DeleteMany([FromQuery] Guid[] ids)
        {
            await _med.Send(new DeleteManyGebruikenCommand(ids));
            return NoContent();
        }
    }
}
