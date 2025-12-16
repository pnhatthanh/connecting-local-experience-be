using BuildingBlocks.Application.CQRS.Query;
using MediatR;
using User.Application.DTOs;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Queries.GetHostsBatch;

public class GetHostsBatchQueryHandler : IQueryHandler<GetHostsBatchQuery, List<HostDto>>
{
    private readonly IUserRepository _userRepository;

    public GetHostsBatchQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<HostDto>> Handle(GetHostsBatchQuery request, CancellationToken cancellationToken)
    {
        if (!request.HostIds.Any())
            return new List<HostDto>();

        var specification = new UserByIdsSpecification(request.HostIds);
        
        var users = await _userRepository.GetAllAsync(specification, u => u.HostProfile!);
        if (users == null || !users.Any())
            return new List<HostDto>();
        var hostDtos = users.Select(u => new HostDto
        {
            Id = u.Id,
            FullName = u.FullName ?? string.Empty,
            TotalExperiences = u.HostProfile?.TotalExperiences ?? 0,
            RatingAvg = u.HostProfile?.RatingAvg ?? 0,
            TotalBookings = u.HostProfile?.TotalBookings ?? 0
        }).ToList();
        return hostDtos;
    }
}
