using Experience.Application.Handlers.Commands.CreateReview;
using Experience.Application.Handlers.Commands.HideReview;
using Experience.Application.Handlers.Queries.GetAllReviews;
using Experience.Application.Handlers.Queries.GetReviewsByExperience;
using Experience.Application.Handlers.Queries.GetReviewsByHost;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Presentation.Authorization;
using BuildingBlocks.Presentation.Constants;

namespace Experience.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("experiences/{experienceId}/reviews")]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromRoute] Guid experienceId, [FromBody] CreateReviewCommand command)
        {
            var result = await _mediator.Send(command with { ExperienceId = experienceId });
            return CreatedAtAction(nameof(CreateReview), new { experienceId }, result);
        }

        [HttpGet("experiences/{experienceId}/reviews")]
        public async Task<IActionResult> GetReviewsByExperience(
            [FromRoute] Guid experienceId, 
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetReviewsByExperienceQuery(experienceId, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("reviews")]
        [RequirePermission(PermissionCodes.EXPERIENCE_REVIEW_VIEW_ALL)]
        public async Task<IActionResult> GetAllReviews([FromQuery] GetAllReviewsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("hosts/{hostId}/reviews")]
        public async Task<IActionResult> GetReviewsByHost( [FromRoute] Guid hostId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetReviewsByHostQuery(hostId, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("reviews/{reviewId}/status")]
        [RequirePermission(PermissionCodes.EXPERIENCE_REVIEW_UPDATE_STATUS)]
        public async Task<IActionResult> HideReview([FromRoute] Guid reviewId, [FromBody] HideReviewCommand command)
        {
            var result = await _mediator.Send(command with { ReviewId = reviewId });
            return Ok(new
            {
                Success = result,
                Message = result ? "Review visibility updated successfully" : "Failed to update review visibility"
            });
        }
    }
}
