using BuildingBlocks.Application.CQRS.Command;

namespace Experience.Application.Handlers.Commands.DeleteExperience
{
    public record DeleteExperienceCommand(Guid Id) : ICommand<bool>;
}
