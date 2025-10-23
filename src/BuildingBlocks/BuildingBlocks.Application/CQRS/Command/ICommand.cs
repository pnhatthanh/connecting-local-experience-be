using MediatR;

namespace BuildingBlocks.Application.CQRS.Command
{
    public interface ICommand : IRequest<Unit>
    {}
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {}
}
