using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Commands.DeleteExperience
{
    public class DeleteExperienceCommandHandler : IRequestHandler<DeleteExperienceCommand, bool>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExperienceCommandHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.Id);
            
            if (experience == null)
            {
                throw new KeyNotFoundException($"Experience with ID {request.Id} not found.");
            }

            if (experience.HostId != request.HostId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this experience.");
            }

            _experienceRepository.Delete(experience);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
