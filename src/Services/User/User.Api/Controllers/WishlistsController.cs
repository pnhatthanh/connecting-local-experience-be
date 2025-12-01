using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Application.DTOs;
using User.Application.Handlers.Commands.AddExperienceToWishlist;
using User.Application.Handlers.Commands.CreateWishlist;
using User.Application.Handlers.Commands.DeleteWishlist;
using User.Application.Handlers.Commands.RemoveExperienceFromWishlist;
using User.Application.Handlers.Queries.CheckExperiencesInWishlist;
using User.Application.Handlers.Queries.GetUserWishlists;
using User.Application.Handlers.Queries.GetWishlistDetail;

namespace User.Api.Controllers
{
    [ApiController]
    [Route("api/wishlists")]
    [Authorize]
    public class WishlistsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WishlistsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<List<WishlistDto>>> GetMyWishlists(CancellationToken cancellationToken)
        {
            var query = new GetUserWishlistsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpGet("{wishlistId}")]
        public async Task<ActionResult<WishlistDetailDto>> GetWishlistDetail(
            [FromRoute] Guid wishlistId,
            CancellationToken cancellationToken)
        {
            var query = new GetWishlistDetailQuery { WishlistId = wishlistId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateWishlist(
            [FromBody] CreateWishlistCommand command,
            CancellationToken cancellationToken)
        {
            var wishlistId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetWishlistDetail),
                new { wishlistId },
                new { Id = wishlistId, isSuccess = true, message = "Wishlist created successfully" });
        }
        [HttpPost("{wishlistId}/experiences/{experienceId}")]
        public async Task<ActionResult> AddExperienceToWishlist(
            [FromRoute] Guid wishlistId,
            [FromRoute] Guid experienceId,
            CancellationToken cancellationToken)
        {
            var command = new AddExperienceToWishlistCommand
            {
                WishlistId = wishlistId,
                ExperienceId = experienceId
            };
            await _mediator.Send(command, cancellationToken);
            return Ok(new {isSuccess = true, message = "Experience added to wishlist successfully" });
        }

        [HttpDelete("{wishlistId}/experiences/{experienceId}")]
        public async Task<ActionResult> RemoveExperienceFromWishlist(
            [FromRoute] Guid wishlistId,
            [FromRoute] Guid experienceId,
            CancellationToken cancellationToken)
        {
            var command = new RemoveExperienceFromWishlistCommand
            {
                WishlistId = wishlistId,
                ExperienceId = experienceId
            };
            await _mediator.Send(command, cancellationToken);
            return Ok(new {isSuccess = true, message = "Experience removed from wishlist successfully" });
        }
        [HttpDelete("{wishlistId}")]
        public async Task<ActionResult> DeleteWishlist(
            [FromRoute] Guid wishlistId,
            CancellationToken cancellationToken)
        {
            var command = new DeleteWishlistCommand { WishlistId = wishlistId };
            await _mediator.Send(command, cancellationToken);
            return Ok(new {isSuccess = true, message = "Wishlist deleted successfully" });
        }

        [HttpGet("check-experiences")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Guid>>> CheckExperiencesInWishlist(
            [FromQuery] Guid userId,
            [FromQuery] List<Guid> experienceIds,
            CancellationToken cancellationToken)
        {
            var query = new CheckExperiencesInWishlistQuery(
                userId, 
                experienceIds);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
