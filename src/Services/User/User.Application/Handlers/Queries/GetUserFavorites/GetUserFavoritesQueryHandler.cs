// using BuildingBlocks.Application.CQRS.Query;
// using BuildingBlocks.Application.Dtos;
// using BuildingBlocks.Domain.Exceptions;
// using MapsterMapper;
// using User.Application.DTOs;
// using User.Domain.Repositories;
// using User.Domain.Specifications;

// namespace User.Application.Handlers.Queries.GetUserFavorites
// {
//     public class GetUserFavoritesQueryHandler : IQueryHandler<GetUserFavoritesQuery, PaginationResult<UserFavoriteExperienceDto>>
//     {
//         private readonly IUserRepository _userRepository;
//         private readonly IUserFavoriteExperienceRepository _favoriteRepository;
//         private readonly IMapper _mapper;

//         public GetUserFavoritesQueryHandler(
//             IUserRepository userRepository,
//             IUserFavoriteExperienceRepository favoriteRepository,
//             IMapper mapper)
//         {
//             _userRepository = userRepository;
//             _favoriteRepository = favoriteRepository;
//             _mapper = mapper;
//         }

//         public async Task<PaginationResult<UserFavoriteExperienceDto>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
//         {
//             var userSpec = new UserByIdSpecification(request.UserId);
//             var user = await _userRepository.GetBySpecAsync(userSpec)
//                 ?? throw new NotFoundException($"User with Id {request.UserId} not found");

//             var favoriteSpec = new UserFavoriteByUserIdSpecification(request.UserId);
//             var favorites = await _favoriteRepository.GetPagedAsync(
//                 favoriteSpec,
//                 request.PageIndex,
//                 request.PageSize);

//             var totalCount = await _favoriteRepository.CountAsync(favoriteSpec);

//             var dtos = _mapper.Map<List<UserFavoriteExperienceDto>>(favorites);

//             return new PaginationResult<UserFavoriteExperienceDto>
//             {
//                 Items = dtos,
//                 TotalCount = totalCount,
//                 PageIndex = request.PageIndex,
//                 PageSize = request.PageSize
//             };
//         }
//     }
// }
