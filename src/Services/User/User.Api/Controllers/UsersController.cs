using BuildingBlocks.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.DTOs;
using User.Application.Handlers.Commands.BecomeHost;
using User.Application.Handlers.Commands.UpdateUserProfile;
using User.Application.Handlers.Queries.GetMyProfile;
using User.Application.Handlers.Queries.GetUserById;
using User.Application.Handlers.Queries.GetUsers;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginationResult<UserDto>>> GetUsers(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? role = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUsersQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Role = role
            };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetUserByIdQuery { UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetMyProfile(CancellationToken cancellationToken)
        {
            var query = new GetMyProfileQuery();
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
            return Ok(result);
        }
    }
}
