using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetHostsBatch;

public class GetHostsBatchQuery : IQuery<List<HostDto>>
{
    public List<Guid> HostIds { get; set; } = new();
}
