using BuildingBlocks.Application.CQRS.Command;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid ExperienceId,
        int Rating,
        string Description
    ) : ICommand<ReviewDto>;
}
