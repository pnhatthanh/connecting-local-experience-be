using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using Experience.Domain.Enums;
using Experience.Domain.Repositories;

namespace Experience.Application.Handlers.Commands.UpdateExperienceStatus
{
    public class UpdateExperienceStatusHandler : ICommandHandler<UpdateExperienceStatusCommand, bool>
    {
        private readonly IExperienceRepository _experienceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExperienceStatusHandler(
            IExperienceRepository experienceRepository,
            IUnitOfWork unitOfWork)
        {
            _experienceRepository = experienceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateExperienceStatusCommand request, CancellationToken cancellationToken)
        {
            var experience = await _experienceRepository.GetByIdAsync(request.ExperienceId)
                ?? throw new BadRequestException($"Experience with ID {request.ExperienceId} not found");

            switch (request.Action.ToLower())
            {
                case "approve":
                    if (experience.Status != ExperienceStatus.Pending)
                        throw new BadRequestException($"Only experiences with Pending status can be approved. Current status: {experience.Status}");
                    experience.Status = ExperienceStatus.Approved;
                    break;

                case "reject":
                    if (experience.Status != ExperienceStatus.Pending)
                        throw new BadRequestException($"Only experiences with Pending status can be rejected. Current status: {experience.Status}");
                    
                    if (string.IsNullOrWhiteSpace(request.Reason))
                        throw new BadRequestException("Reason is required when rejecting an experience");
                    
                    experience.Status = ExperienceStatus.Rejected;
                    break;

                case "lock":
                    if (experience.Status == ExperienceStatus.Locked)
                        throw new BadRequestException("Experience is already locked");
                    
                    if (string.IsNullOrWhiteSpace(request.Reason))
                        throw new BadRequestException("Reason is required when locking an experience");
                    
                    experience.Status = ExperienceStatus.Locked;
                    break;
                case "unlock":
                    if (experience.Status != ExperienceStatus.Locked)
                        throw new BadRequestException("Only locked experiences can be unlocked");
                    
                    experience.Status = ExperienceStatus.Approved;
                    break;

                default:
                    throw new BadRequestException($"Invalid action: {request.Action}. Valid actions are: Approve, Reject, Lock");
            }

            experience.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
