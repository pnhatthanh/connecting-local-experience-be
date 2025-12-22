using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.DeleteWishlist
{
    public class DeleteWishlistCommandHandler : ICommandHandler<DeleteWishlistCommand, bool>
    {
        private readonly IUserWishlistRepository _wishlistRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteWishlistCommandHandler(
            IUserWishlistRepository wishlistRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _wishlistRepository = wishlistRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var wishlist = await _wishlistRepository.GetByIdAsync(request.WishlistId)
                ?? throw new BadRequestException($"Wishlist with Id {request.WishlistId} not found");
            if (wishlist.UserId != userId)
                throw new ForbiddenException("You don't have permission to delete this wishlist");

            _wishlistRepository.Delete(wishlist);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
