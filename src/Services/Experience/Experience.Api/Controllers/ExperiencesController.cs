using Experience.Application.Handlers.Commands.CreateExperience;
using Experience.Application.Handlers.Commands.DeleteExperience;
using Experience.Application.Handlers.Commands.UpdateExperience;
using Experience.Application.Handlers.Commands.UpdateExperienceStatus;
using Experience.Application.Handlers.Queries.GetAllExperiencesForAdmin;
using Experience.Application.Handlers.Queries.GetExperience;
using Experience.Application.Handlers.Queries.GetExperienceAvailability;
using Experience.Application.Handlers.Queries.GetExperiences;
using Experience.Application.Handlers.Queries.GetExperiencesByIds;
using Experience.Application.Handlers.Queries.GetRecommendations;
using Experience.Application.Handlers.Queries.ValidateBookingAvailability;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Presentation.Authorization;
using BuildingBlocks.Presentation.Constants;
using Microsoft.AspNetCore.Authorization;

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
        [RequirePermission(PermissionCodes.EXPERIENCE_EXPERIENCE_CREATE)]
        public async Task<IActionResult> CreateExperience([FromForm] CreateExperienceCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetExperience), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [RequirePermission(PermissionCodes.EXPERIENCE_EXPERIENCE_UPDATE)]
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

        [HttpGet("admin/all")]
        [RequirePermission(PermissionCodes.EXPERIENCE_EXPERIENCE_VIEW_ADMIN)]
        public async Task<IActionResult> GetAllExperiencesForAdmin(
            [FromQuery] GetAllExperiencesForAdminQuery query,
            CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(query, cancellationToken);
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
        public async Task<IActionResult> GetExperienceAvailability([FromRoute] Guid id, 
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate)
        {
            var query = new GetExperienceAvailabilityQuery(id, startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}/validate-booking")]
        public async Task<IActionResult> ValidateBookingAvailability(
            [FromRoute] Guid id,
            [FromQuery] DateOnly date,
            [FromQuery] TimeSpan startTime,
            [FromQuery] TimeSpan endTime,
            [FromQuery] int adults,
            [FromQuery] int children)
        {
            var query = new ValidateBookingAvailabilityQuery(id, date, startTime, endTime, adults, children);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [RequirePermission(PermissionCodes.EXPERIENCE_EXPERIENCE_UPDATE_STATUS)]
        public async Task<IActionResult> UpdateExperienceStatus([FromRoute] Guid id, [FromBody] UpdateExperienceStatusCommand command)
        {
            var result = await _mediator.Send(command with { ExperienceId = id });
            
            var message = command.Action.ToLower() switch
            {
                "approve" => "Experience approved successfully.",
                "reject" => "Experience rejected successfully.",
                "lock" => "Experience locked successfully.",
                _ => "Experience status updated successfully."
            };
                
            return Ok(new
            {
                Success = result,
                Message = message
            });
        }

        [HttpDelete("{id}")]
        [RequirePermission(PermissionCodes.EXPERIENCE_EXPERIENCE_DELETE)]
        public async Task<IActionResult> DeleteExperience([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteExperienceCommand(id));
            return Ok(new
            {
                Success = result,
                Message = result ? "Experience deleted successfully." : "Failed to delete experience."
            });
        }
        [HttpGet("recommendations")]
        [Authorize]
        public async Task<IActionResult> GetRecommendations( [FromQuery] int topK = 10)
        {
            var query = new GetRecommendationsQuery(topK);
            var result = await _mediator.Send(query);
            
            return Ok(new
            {
                Recommendations = result,
                Total = result.Count
            });
        }
    }
}
