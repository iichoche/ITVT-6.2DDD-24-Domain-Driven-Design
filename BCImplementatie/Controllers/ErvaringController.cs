using BCImplementatie.Application.Commands.Ervaringen;
using BCImplementatie.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;






namespace BCImplementatie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErvaringenController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ErvaringenController(IMediator mediator)
            => _mediator = mediator;

        // GET api/ervaringen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ErvaringDto>>> GetAll(CancellationToken ct)
        {
            var list = await _mediator.Send(new GetAllErvaringenQuery(), ct);
            return Ok(list);
        }

        // GET api/ervaringen/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ErvaringDto>> GetById(int id, CancellationToken ct)
        {
            var item = await _mediator.Send(new GetErvaringByIdQuery(id), ct);
            if (item is null) return NotFound();
            return Ok(item);
        }

        // POST api/ervaringen
        [HttpPost]
        public async Task<ActionResult<ErvaringDto>> Create(
            [FromBody] CreateErvaringDto dto,
            CancellationToken ct)
        {
            var created = await _mediator.Send(
                new AddErvaringCommand(dto.GebruikId, dto.Datum, dto.Review, dto.Observatie),
                ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/ervaringen/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ErvaringDto>> Update(
            int id,
            [FromBody] UpdateErvaringDto dto,
            CancellationToken ct)
        {
            var updated = await _mediator.Send(
                new UpdateErvaringCommand(id, dto.Datum, dto.Review, dto.Observatie),
                ct);
            if (updated is null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/ervaringen/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteErvaringCommand(id), ct);
            return NoContent();
        }

        // DELETE api/ervaringen?ids=1,2,3
        [HttpDelete]
        public async Task<IActionResult> DeleteMany(
            [FromQuery] int[] ids,
            CancellationToken ct)
        {
            await _mediator.Send(new DeleteManyErvaringenCommand(ids), ct);
            return NoContent();
        }
    }
}

