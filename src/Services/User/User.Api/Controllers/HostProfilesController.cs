using BuildingBlocks.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.DTOs;
using User.Application.Handlers.Commands.VerifyHost;
using User.Application.Handlers.Queries.GetHostDetail;
using User.Application.Handlers.Queries.GetHosts;

namespace User.Api.Controllers
{
    [ApiController]
    [Route("api/hosts")]
    public class HostProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HostProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginationResult<HostSummaryDto>>> GetHosts(
            [FromQuery] GetHostsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<HostDetailDto>> GetHostDetail(
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var query = new GetHostDetailQuery { HostId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{accountId}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HostProfileDto>> VerifyHostProfile(
            [FromRoute] Guid accountId,
            [FromBody] VerifyHostCommand command,
            CancellationToken cancellationToken)
        {
            command.AccountId = accountId;
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
