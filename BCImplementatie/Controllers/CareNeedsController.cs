using BCImplementatie.Application.Commands.CareNeeds;
using BCImplementatie.Application.DTOs;
using BCImplementatie.Application.Queries.CareNeeds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BCImplementatie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CareNeedsController : ControllerBase
    {
        private readonly IMediator _med;
        public CareNeedsController(IMediator med) => _med = med;

        // GET api/care-needs
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _med.Send(new GetAllCareNeedsQuery()));

        // GET api/care-needs/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
            => Ok(await _med.Send(new GetCareNeedByIdQuery(id)));

        // POST api/care-needs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCareNeedDto dto)
        {
            var entity = await _med.Send(new AddCareNeedCommand(
                dto.GebruikId, dto.NeedDescription, dto.NeedCategoryName, dto.AdviesProductId));
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        // PUT api/care-needs/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCareNeedDto dto)
        {
            var updated = await _med.Send(new UpdateCareNeedCommand(
                id, dto.NeedDescription, dto.NeedCategoryName, dto.AdviesProductId));
            return Ok(updated);
        }

        // DELETE api/care-needs/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _med.Send(new DeleteCareNeedCommand(id));
            return NoContent();
        }

        // DELETE api/care-needs?ids=1,2,3
        [HttpDelete]
        public async Task<IActionResult> DeleteMany([FromQuery] int[] ids)
        {
            await _med.Send(new DeleteManyCareNeedsCommand(ids));
            return NoContent();
        }
    }
}

