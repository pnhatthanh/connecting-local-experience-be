using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Interfaces;
using MapsterMapper;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetUserWishlists
{
    public class GetUserWishlistsQueryHandler : IQueryHandler<GetUserWishlistsQuery, List<WishlistDto>>
    {
        private readonly IUserWishlistRepository _wishlistRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetUserWishlistsQueryHandler(IUserWishlistRepository wishlistRepository, ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<List<WishlistDto>> Handle(GetUserWishlistsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var wishlistSpec = new WishlistByUserIdSpecification(userId);
            var wishlists = await _wishlistRepository.GetAllAsync(wishlistSpec);
            return _mapper.Map<List<WishlistDto>>(wishlists);
        }
    }
}
