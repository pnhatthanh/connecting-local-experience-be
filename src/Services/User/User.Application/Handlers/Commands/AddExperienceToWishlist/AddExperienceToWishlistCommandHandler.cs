using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using User.Application.Interfaces;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.AddExperienceToWishlist
{
    public class AddExperienceToWishlistCommandHandler : ICommandHandler<AddExperienceToWishlistCommand, bool>
    {
        private readonly IUserWishlistRepository _wishlistRepository;
        private readonly IWishlistExperienceRepository _wishlistExperienceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExperienceServiceClient _experienceServiceClient;

        public AddExperienceToWishlistCommandHandler(
            IUserWishlistRepository wishlistRepository,
            IWishlistExperienceRepository wishlistExperienceRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IExperienceServiceClient experienceServiceClient)
        {
            _wishlistRepository = wishlistRepository;
            _wishlistExperienceRepository = wishlistExperienceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _experienceServiceClient = experienceServiceClient;
        }

        public async Task<bool> Handle(AddExperienceToWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var wishlist = await _wishlistRepository.GetByIdAsync(request.WishlistId)
                ?? throw new BadRequestException($"Wishlist with Id {request.WishlistId} not found");
            if (wishlist.UserId != userId)
                throw new ForbiddenException("You don't have permission to modify this wishlist");

            var experienceExists = await _experienceServiceClient.CheckExperienceExistsAsync(request.ExperienceId);
            if (!experienceExists)
                throw new BadRequestException($"Experience with Id {request.ExperienceId} not found");

            var existingSpec = new WishlistByExperienceIdSpecification(request.WishlistId, request.ExperienceId);
            if (await _wishlistExperienceRepository.CheckExistsAsync(existingSpec))
                throw new BadRequestException("Experience already exists in this wishlist");

            var wishlistExperience = new WishlistExperienceEntity
            {
                Id = Guid.NewGuid(),
                WishlistId = request.WishlistId,
                ExperienceId = request.ExperienceId,
                CreatedAt = DateTime.UtcNow
            };
            await _wishlistExperienceRepository.AddAsync(wishlistExperience);
            
            wishlist.ExperienceCount++;
            _wishlistRepository.Update(wishlist);
            
            await _unitOfWork.SaveChangeAsync();
            return true;
        }
    }
}
