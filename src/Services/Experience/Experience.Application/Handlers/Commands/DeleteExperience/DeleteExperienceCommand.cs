using MediatR;

namespace Experience.Application.Handlers.Commands.DeleteExperience
{
    public class DeleteExperienceCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; } 
    }
}
