using MediatR;

namespace BuildingBlocks.Application.CQRS.Query
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}

