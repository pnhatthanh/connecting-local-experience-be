using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using MapsterMapper;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IQueryHandler<GetMyProfileQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetMyProfileQueryHandler(
            IUserRepository userRepository, 
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            
            if (userId == Guid.Empty)
                throw new UnAuthorizedException("User is not authenticated");

            var userSpec = new UserByIdSpecification(userId);
            var user = await _userRepository.GetBySpecAsync(userSpec, user => user.HostProfile!)
                ?? throw new BadRequestException($"User with Id {userId} not found");
            
            return _mapper.Map<UserDto>(user);
        }
    }
}
