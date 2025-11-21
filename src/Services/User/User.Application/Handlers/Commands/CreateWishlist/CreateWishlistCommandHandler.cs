using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.CreateWishlist
{
    public class CreateWishlistCommandHandler : ICommandHandler<CreateWishlistCommand, Guid>
    {
        private readonly IUserWishlistRepository _wishlistRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateWishlistCommandHandler(
            IUserWishlistRepository wishlistRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _wishlistRepository = wishlistRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateWishlistCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if(await _userRepository.CheckExistsAsync(new UserByIdSpecification(userId)) == false)
                throw new UnAuthorizedException($"User with Id {userId} not found");
            if (await _wishlistRepository.CheckExistsAsync(new WishlistByNameSpecification(userId, request.Name)))
                throw new BadRequestException($"Wishlist with name '{request.Name}' already exists");

            var wishlist = new UserWishlistEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow
            };

            await _wishlistRepository.AddAsync(wishlist);
            await _unitOfWork.SaveChangeAsync();

            return wishlist.Id;
        }
    }
}
