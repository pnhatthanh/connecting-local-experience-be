using MediatR;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Application.Dtos;
using UserService.Application.DTOs.Admin;
using UserService.Application.Queries.Admin.ListUsers;
using UserService.Application.Queries.Admin.GetUserDetail;
using UserService.Application.Commands.Admin.DeactivateUser;
using UserService.Application.Commands.Admin.ActivateUser;

namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/admin/users
        [HttpGet("users")]
        public async Task<ActionResult<PaginationResult<AdminUserListItem>>> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null, [FromQuery] bool isAscending = true, [FromQuery] string? keyword = null, [FromQuery] bool? isActive = null, [FromQuery] string? roleName = null)
        {
            var result = await _mediator.Send(new ListUsersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                IsAscending = isAscending,
                Keyword = keyword,
                IsActive = isActive,
                RoleName = roleName
            });
            return Ok(result);
        }

        // GET /api/admin/users/{userId}
        [HttpGet("users/{userId:guid}")]
        public async Task<ActionResult<AdminUserDetailResponse>> GetDetail([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUserDetailQuery(userId));
            return Ok(result);
        }

        // PUT /api/admin/users/{userId}/deactivate
        [HttpPut("users/{userId:guid}/deactivate")]
        public async Task<ActionResult> Deactivate([FromRoute] Guid userId)
        {
            var ok = await _mediator.Send(new DeactivateUserCommand(userId));
            if (!ok)
            {
                return NotFound(new { success = false, message = "Account not found or already deactivated" });
            }
            return Ok(new { success = true });
        }

        // PUT /api/admin/users/{userId}/activate
        [HttpPut("users/{userId:guid}/activate")]
        public async Task<ActionResult> Activate([FromRoute] Guid userId)
        {
            var ok = await _mediator.Send(new ActivateUserCommand(userId));
            if (!ok)
            {
                return NotFound(new { success = false, message = "Account not found" });
            }
            return Ok(new { success = true });
        }
    }
}
