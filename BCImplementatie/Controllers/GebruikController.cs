using MediatR;
using Microsoft.AspNetCore.Mvc;
using BCImplementatie.Application.Commands.CareNeeds;
using BCImplementatie.Application.Commands.Ervaringen;
using BCImplementatie.Application.Commands.StartGebruik;
using BCImplementatie.Application.DTOs;
using BCImplementatie.Application.Queries.Gebruiken;


namespace BCImplementatie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GebruikController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GebruikController(IMediator mediator) => _mediator = mediator;

        // POST /api/gebruik
        [HttpPost]
        public async Task<IActionResult> Start([FromBody] StartGebruikDto dto)
        {
            var id = await _mediator.Send(new StartGebruikCommand(dto.ClientId, dto.ProductItemId));
            return CreatedAtAction(nameof(GetById), new { id }, new { gebruikId = id });
        }

        // GET /api/gebruik
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _mediator.Send(new GetAllGebruikQuery()));

        // GET /api/gebruik/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
            => Ok(await _mediator.Send(new GetGebruikByIdQuery(id)));

        // GET care-needs (1)
        // GET care-needs (meerdere)
        // POST care-needs (nieuwe entry)
        // UPDATE care-needs (bestaande entry)
        // DELETE care-needs (bestaande entry(s))

        // GET ervaringen (1)
        // GET ervaringen (meerdere)
        // POST ervaringen (nieuwe entry)
        // UPDATE ervaringen (bestaande entry)
        // DELETE ervaringen (bestaande entry(s))

        // GET gebruiken (1)
        // GET gebruiken (meerdere)
        // POST gebruiken (nieuwe entry)
        // UPDATE gebruiken (bestaande entry)
        // DELETE gebruiken (bestaande entry(s))


        //Volgende mogen weghalen, deze zijn al in de controller aanwezig:

    //    // POST /api/gebruik/{id}/care-needs
    //    [HttpPost("{id:guid}/care-needs")]
    //    public async Task<IActionResult> AddNeed(Guid id, [FromBody] AddCareNeedDto dto)
    //    {
    //        var need = await _mediator.Send(new AddCareNeedCommand(id,
    //            dto.NeedDescription, dto.NeedCategoryName, dto.AdviesProductId));
    //        return Ok(need);
    //    }

    //    // POST /api/gebruik/{id}/ervaringen
    //    [HttpPost("{id:guid}/ervaringen")]
    //    public async Task<IActionResult> AddErvaring(Guid id, [FromBody] AddErvaringDto dto)
    //    {
    //        var erv = await _mediator.Send(new AddErvaringCommand(id, dto.Datum, dto.Review, dto.Observatie));
    //        return Ok(erv);
    //    }
    }
}
