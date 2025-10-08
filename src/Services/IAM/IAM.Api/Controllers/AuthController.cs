using IAM.Application.Handlers.Commands.RegisterCommand;
using IAM.Application.Handlers.Commands.LoginCommand;
using IAM.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IAM.Application.Handlers.Commands.RefreshTokenCommand;
using IAM.Application.Handlers.Commands.ConfirmEmailCommand;
using IAM.Application.Handlers.Commands.ForgotPasswordCommand;
using IAM.Application.Handlers.Commands.ResetPasswordCommand;

namespace IAM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("confirm-email")]
        public async Task<ActionResult<bool>> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Email confirmed successfully" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? "";
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token is required" });
            var command = new RefreshTokenCommand { RefreshToken = refreshToken };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<bool>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Password reset email has been sent" });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result, message = "Password has been reset successfully" });
        }
    }
}