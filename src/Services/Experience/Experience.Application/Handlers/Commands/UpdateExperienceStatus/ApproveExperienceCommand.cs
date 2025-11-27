using BuildingBlocks.Application.CQRS.Command;

namespace Experience.Application.Handlers.Commands.UpdateExperienceStatus
{
    public record UpdateExperienceStatusCommand(
        Guid ExperienceId, 
        string Action, // "Approve", "Reject", "Lock"
        string? Reason = null) : ICommand<bool>;
}
