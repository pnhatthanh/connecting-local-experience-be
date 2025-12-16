using BuildingBlocks.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.DTOs;
using BuildingBlocks.Presentation.Authorization;
using BuildingBlocks.Presentation.Constants;
using User.Application.Handlers.Commands.VerifyHost;
using User.Application.Handlers.Queries.GetHostDetail;
using User.Application.Handlers.Queries.GetHosts;
using User.Application.Handlers.Queries.GetHostsBatch;

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
        [RequirePermission(PermissionCodes.USER_HOST_VIEW_ALL)]
        public async Task<ActionResult<PaginationResult<HostSummaryDto>>> GetHosts(
            [FromQuery] GetHostsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("batch")]
        [AllowAnonymous]
        public async Task<ActionResult<List<HostProfileDto>>> GetHostsBatch(
            [FromQuery] List<Guid> ids,
            CancellationToken cancellationToken)
        {
            var query = new GetHostsBatchQuery { HostIds = ids };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<HostDetailDto>> GetHostDetail(
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var query = new GetHostDetailQuery { HostId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{accountId}/verify")]
        [RequirePermission(PermissionCodes.USER_HOST_VERIFY)]
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
