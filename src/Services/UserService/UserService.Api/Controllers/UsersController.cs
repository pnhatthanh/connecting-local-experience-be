using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands.DeleteCurrentUser;
using UserService.Application.Commands.UpdateUserProfile;
using UserService.Application.DTOs;
using UserService.Application.Queries.GetCurrentUser;
using UserService.Application.Queries.GetUserById;

namespace UserService.Api.Controllers
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

        /// <summary>
        /// GET /api/users/{accountId} - Lấy thông tin chi tiết của một người dùng theo Account ID
        /// </summary>
        [HttpGet("{accountId:guid}")]
        public async Task<ActionResult<UserProfileResponse>> GetUserById([FromRoute] Guid accountId)
        {
            var query = new GetUserByIdQuery(accountId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// GET /api/users/me - Lấy thông tin của chính user đang đăng nhập
        /// </summary>
        [HttpGet("me")]
        // [Authorize] // Temporarily disabled for testing
        public async Task<ActionResult<UserProfileResponse>> GetCurrentUser()
        {
            var query = new GetCurrentUserQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// PUT /api/users/me - Cập nhật thông tin cá nhân
        /// </summary>
        [HttpPut("me")]
        // [Authorize] // Temporarily disabled for testing
        public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            var command = new UpdateUserProfileCommand
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Nationality = request.Nationality,
                AvatarUrl = request.AvatarUrl
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// DELETE /api/users/me - Vô hiệu hóa tài khoản của chính mình
        /// </summary>
        [HttpDelete("me")]
        // [Authorize] // Temporarily disabled for testing
        public async Task<ActionResult> DeleteCurrentUser()
        {
            var command = new DeleteCurrentUserCommand();
            var result = await _mediator.Send(command);
            
            if (result)
            {
                return Ok(new { 
                    success = true,
                    message = "Your account has been deactivated successfully" 
                });
            }
            
            return BadRequest(new { 
                success = false,
                message = "Account not found or already deactivated" 
            });
        }
    }
}