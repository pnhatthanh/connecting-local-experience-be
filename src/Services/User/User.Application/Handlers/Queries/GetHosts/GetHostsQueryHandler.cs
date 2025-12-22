using BuildingBlocks.Application.CQRS.Query;
using BuildingBlocks.Application.Dtos;
using MapsterMapper;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetHosts;

public class GetHostsQueryHandler : IQueryHandler<GetHostsQuery, PaginationResult<HostSummaryDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetHostsQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResult<HostSummaryDto>> Handle(GetHostsQuery request, CancellationToken cancellationToken)
    {
        var spec = new HostFilterSpecification(request.SearchTerm, request.Status);
        
        var users = await _userRepository.GetPagedListAsync(
            spec,
            request.PageIndex,
            request.PageSize,
            includes: u => u.HostProfile!);

        var totalCount = await _userRepository.CountAsync(spec);

        var dtos = _mapper.Map<List<HostSummaryDto>>(users);

        return new PaginationResult<HostSummaryDto>
        {
            Data = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}