using BuildingBlocks.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.DTOs;
using User.Application.Handlers.Commands.AddFavorite;
using User.Application.Handlers.Commands.BecomeHost;
using User.Application.Handlers.Commands.RemoveFavorite;
using User.Application.Handlers.Commands.UpdateUserProfile;
using User.Application.Handlers.Queries.GetUserById;
using User.Application.Handlers.Queries.GetUserFavorites;

namespace User.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile([FromForm] UpdateUserProfileCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        
        [HttpPost("become-host")]
        [Authorize]
        public async Task<ActionResult<HostProfileDto>> BecomeHost( [FromForm] BecomeHostCommand command, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserById), new { userId = result.UserId }, result);
        }

        [HttpGet("{userId:guid}/favorites")]
        [Authorize]
        public async Task<IActionResult> GetUserFavorites([FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetUserFavoritesQuery 
            { 
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpPost("favorites/{experienceId}")]
        [Authorize]
        public async Task<ActionResult<bool>> AddFavorite([FromRoute]Guid experienceId, CancellationToken cancellationToken)
        {
            var command = new AddFavoriteCommand 
            {
                ExperienceId = experienceId 
            };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("favorites/{experienceId}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveFavorite([FromRoute]Guid experienceId, CancellationToken cancellationToken)
        {
            var command = new RemoveFavoriteCommand
            {
                ExperienceId = experienceId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
