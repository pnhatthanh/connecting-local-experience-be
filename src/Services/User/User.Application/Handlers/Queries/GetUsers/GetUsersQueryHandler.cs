using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using MapsterMapper;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetUsers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PaginationResult<UserSummaryDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<UserSummaryDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var spec = new UserFilterSpecification(request.SearchTerm, request.Role, request.Status);
            
            var users = await _userRepository.GetPagedListAsync(
                spec,
                request.PageIndex,
                request.PageSize);

            var totalCount = await _userRepository.CountAsync(spec);

            var dtos = _mapper.Map<List<UserSummaryDto>>(users);

            return new PaginationResult<UserSummaryDto>
            {
                Data = dtos,
                TotalCount = totalCount,
                PageNumber = request.PageIndex,
                PageSize = request.PageSize
            };
        }
    }
}
