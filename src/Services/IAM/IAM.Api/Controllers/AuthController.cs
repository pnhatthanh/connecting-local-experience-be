using IAM.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IAM.Application.Handlers.Commands.ConfirmEmail;
using IAM.Application.Handlers.Commands.Register;
using IAM.Application.Handlers.Commands.Login;
using IAM.Application.Handlers.Commands.RefreshToken;
using IAM.Application.Handlers.Commands.ForgotPassword;
using IAM.Application.Handlers.Commands.ResetPassword;
using IAM.Application.Handlers.Commands.UpdateAccountStatus;
using Microsoft.AspNetCore.Authorization;
namespace IAM.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<bool>> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var command = new ConfirmEmailCommand 
            { 
                Email = email, 
                Token = token 
            };
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Email confirmed successfully" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("password/forgot")]
        public async Task<ActionResult<bool>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Password reset email has been sent" });
        }

        [HttpPost("password/reset")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Password has been reset successfully" });
        }

        [HttpPatch("accounts/{accountId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateAccountStatus(
            [FromRoute] Guid accountId,
            [FromBody] UpdateAccountStatusRequest request)
        {
            var command = new UpdateAccountStatusCommand 
            { 
                AccountId = accountId,
                IsActive = request.IsActive
            };
            var result = await _mediator.Send(command);
            var message = request.IsActive 
                ? "Account has been activated successfully" 
                : "Account has been deactivated successfully";
            return Ok(new { success = result, message });
        }
    }

    public class UpdateAccountStatusRequest
    {
        public bool IsActive { get; set; }
    }
}