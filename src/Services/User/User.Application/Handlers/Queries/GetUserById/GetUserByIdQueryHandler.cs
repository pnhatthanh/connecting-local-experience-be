using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using MapsterMapper;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(request.UserId);
            var user = await _userRepository.GetBySpecAsync(userSpec, user => user.HostProfile!)
                ?? throw new BadRequestException($"User with Id {request.UserId} not found");
            return _mapper.Map<UserDto>(user);
        }
    }
}
