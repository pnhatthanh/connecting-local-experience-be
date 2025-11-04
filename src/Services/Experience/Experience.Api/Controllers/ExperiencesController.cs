using BuildingBlocks.Application.Dtos;
using Experience.Application.Dtos;
using Experience.Application.Handlers.Commands.CreateExperience;
using Experience.Application.Handlers.Commands.DeleteExperience;
using Experience.Application.Handlers.Commands.UpdateExperience;
using Experience.Application.Handlers.Queries.GetExperience;
using Experience.Application.Handlers.Queries.GetExperiences;
using Experience.Application.Handlers.Queries.GetExperiencesByHost;
using Experience.Application.Handlers.Queries.GetNearbyExperiences;
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
        public async Task<ActionResult<ExperienceDto>> CreateExperience([FromForm] CreateExperienceCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetExperience), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<ActionResult<ExperienceDto>> UpdateExperience(Guid id, [FromForm] UpdateExperienceCommand command)
        {
            var result = await _mediator.Send(command with { ExperienceId = id });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExperienceDto>> GetExperience([FromRoute] Guid id)
        {
            var query = new GetExperienceQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResult<ExperienceDto>>> GetExperiences(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] string? status = null)
        {
            var query = new GetExperiencesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Category = category,
                Status = status
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("host/{hostId}")]
        public async Task<ActionResult<List<ExperienceDto>>> GetExperiencesByHost(Guid hostId)
        {
            var query = new GetExperiencesByHostQuery { HostId = hostId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("nearby")]
        public async Task<ActionResult<List<ExperienceDto>>> GetNearbyExperiences(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusInKm = 10)
        {
            var query = new GetNearbyExperiencesQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusInKm = radiusInKm
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Host")]
        public async Task<ActionResult<bool>> DeleteExperience([FromRoute] Guid id)
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
