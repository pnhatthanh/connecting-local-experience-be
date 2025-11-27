using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Domain.Exceptions;
using MapsterMapper;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetHostDetail
{
    public class GetHostDetailQueryHandler : IQueryHandler<GetHostDetailQuery, HostDetailDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetHostDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HostDetailDto> Handle(GetHostDetailQuery request, CancellationToken cancellationToken)
        {
            var spec = new UserByIdSpecification(request.HostId);
            var user = await _userRepository.GetBySpecAsync(spec, includes: u => u.HostProfile!) 
                ?? throw new NotFoundException("User not found");
            if (user.HostProfile == null)
                throw new NotFoundException("This user is not a host");

            var dto = _mapper.Map<HostDetailDto>(user);
            return dto;
        }
    }
}
