using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using User.Application.DTOs;
using User.Application.Interfaces;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetWishlistDetail
{
    public class GetWishlistDetailQueryHandler : IQueryHandler<GetWishlistDetailQuery, WishlistDetailDto>
    {
        private readonly IUserWishlistRepository _wishlistRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExperienceServiceClient _experienceServiceClient;

        public GetWishlistDetailQueryHandler(
            IUserWishlistRepository wishlistRepository,
            ICurrentUserService currentUserService,
            IExperienceServiceClient experienceServiceClient)
        {
            _wishlistRepository = wishlistRepository;
            _currentUserService = currentUserService;
            _experienceServiceClient = experienceServiceClient;
        }

        public async Task<WishlistDetailDto> Handle(GetWishlistDetailQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var wishlistExperienceSpec = new WishlistByIdSpecification(request.WishlistId);
            var wishlist = await _wishlistRepository.GetBySpecAsync(wishlistExperienceSpec, w => w.WishlistExperiences)
                ?? throw new BadRequestException($"Wishlist with Id {request.WishlistId} not found");
            if (wishlist.UserId != userId)
                throw new ForbiddenException("You don't have permission to view this wishlist");
            var experienceIds = wishlist.WishlistExperiences.Select(we => we.ExperienceId).ToList();
            var experienceDetails = await _experienceServiceClient.GetExperiencesByIdsAsync(experienceIds);
            return new WishlistDetailDto(
                wishlist.Id,
                wishlist.Name,
                wishlist.CreatedAt,
                wishlist.UpdatedAt,
                experienceDetails
            );
        }
    }
}
