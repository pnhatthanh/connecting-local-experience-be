using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.DTOs;
using User.Application.Handlers.Commands.VerifyHost;

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
