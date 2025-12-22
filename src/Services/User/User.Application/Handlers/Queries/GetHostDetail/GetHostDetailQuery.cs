using BuildingBlocks.Application.CQRS.Query;
using User.Application.DTOs;

namespace User.Application.Handlers.Queries.GetHostDetail
{
    public class GetHostDetailQuery : IQuery<HostDetailDto>
    {
        public Guid HostId { get; set; }
    }
}