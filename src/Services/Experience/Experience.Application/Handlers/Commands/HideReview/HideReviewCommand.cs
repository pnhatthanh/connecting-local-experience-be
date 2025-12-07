using BuildingBlocks.Application.CQRS.Command;

namespace Experience.Application.Handlers.Commands.HideReview
{
    public record HideReviewCommand(
        Guid ReviewId,
        bool IsHidden
    ) : ICommand<bool>;
}
