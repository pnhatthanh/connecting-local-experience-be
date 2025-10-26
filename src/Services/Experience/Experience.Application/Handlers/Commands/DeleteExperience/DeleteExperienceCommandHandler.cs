using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Repositories;
using MediatR;

namespace Experience.Application.Handlers.Commands.DeleteExperience
{
    public class DeleteExperienceCommandHandler : IRequestHandler<DeleteExperienceCommand, bool>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork; 
        private readonly ICurrentUserService _currentUserService;

        public DeleteExperienceCommandHandler(IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.Id) 
                ?? throw new KeyNotFoundException($"Experience with ID {request.Id} not found.");
            if (experience.HostId != _currentUserService.UserId)
                throw new ForbiddenException("You are not authorized to delete this experience.");
            _experienceRepository.Delete(experience);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }
    }
}
