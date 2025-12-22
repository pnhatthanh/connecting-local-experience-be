using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.RemoveExperienceFromWishlist
{
    public class RemoveExperienceFromWishlistCommandHandler : ICommandHandler<RemoveExperienceFromWishlistCommand, bool>
    {
        private readonly IUserWishlistRepository _wishlistRepository;
        private readonly IWishlistExperienceRepository _wishlistExperienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveExperienceFromWishlistCommandHandler(
            IUserWishlistRepository wishlistRepository,
            IWishlistExperienceRepository wishlistExperienceRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _wishlistRepository = wishlistRepository;
            _wishlistExperienceRepository = wishlistExperienceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(RemoveExperienceFromWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var wishlist = await _wishlistRepository.GetByIdAsync(request.WishlistId)
                ?? throw new BadRequestException($"Wishlist with Id {request.WishlistId} not found");
            if (wishlist.UserId != userId)
                throw new ForbiddenException("You don't have permission to modify this wishlist");
            var wishlistExperienceSpec = new WishlistByExperienceIdSpecification(request.WishlistId, request.ExperienceId);
            var wishlistExperience = await _wishlistExperienceRepository.GetBySpecAsync(wishlistExperienceSpec)
                ?? throw new BadRequestException("Experience not found in this wishlist");
            if (wishlist.ExperienceCount >= 1)
                wishlist.ExperienceCount--;
            _wishlistRepository.Update(wishlist);

            _wishlistExperienceRepository.Delete(wishlistExperience);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
