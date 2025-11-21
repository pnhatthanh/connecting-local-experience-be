using Experience.Application.Handlers.Commands.CreateExperience;
using Experience.Application.Handlers.Commands.DeleteExperience;
using Experience.Application.Handlers.Commands.UpdateExperience;
using Experience.Application.Handlers.Queries.GetExperience;
using Experience.Application.Handlers.Queries.GetExperienceAvailability;
using Experience.Application.Handlers.Queries.GetExperiences;
using Experience.Application.Handlers.Queries.GetExperiencesByIds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Experience.Api.Controllers
{
    [ApiController]
    [Route("api/experiences")]
    public class ExperiencesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExperiencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> CreateExperience([FromForm] CreateExperienceCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetExperience), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> UpdateExperience(Guid id, [FromForm] UpdateExperienceCommand command)
        {
            var result = await _mediator.Send(command with { ExperienceId = id });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetExperiences([FromQuery] GetExperiencesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("batch")]
        public async Task<IActionResult> GetExperiencesByIds([FromQuery] List<Guid> ids)
        {
            var query = new GetExperiencesByIdsQuery(ids);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExperience([FromRoute] Guid id)
        {
            var query = new GetExperienceQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}/availability")]
        public async Task<IActionResult> GetExperienceAvailability(
            [FromRoute] Guid id,
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            var query = new GetExperienceAvailabilityQuery(id, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<IActionResult> DeleteExperience([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteExperienceCommand(id));
            return Ok(new
            {
                Success = result,
                Message = result ? "Experience deleted successfully." : "Failed to delete experience."
            });
        }
    }
}
