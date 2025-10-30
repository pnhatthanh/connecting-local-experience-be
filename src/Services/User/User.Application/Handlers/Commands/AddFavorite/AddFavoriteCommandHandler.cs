using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using User.Domain.Entities;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.AddFavorite
{
    public class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFavoriteExperienceRepository _favoriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddFavoriteCommandHandler(
            IUserRepository userRepository,
            IUserFavoriteExperienceRepository favoriteRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _favoriteRepository = favoriteRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
        {
            var userSpec = new UserByIdSpecification(_currentUserService.UserId);
            var user = await _userRepository.GetBySpecAsync(userSpec)
                ?? throw new NotFoundException($"User with Id {_currentUserService.UserId} not found");

            var existingFavoriteSpec = new UserFavoriteByUserAndExperienceSpecification(_currentUserService.UserId, request.ExperienceId);
            var existingFavorite = await _favoriteRepository.CheckExistsAsync(existingFavoriteSpec);
            if (existingFavorite)
                throw new BadRequestException("Experience is already in favorites");
            var favorite = new UserFavoriteExperienceEntity
            {
                Id = Guid.NewGuid(),
                UserId = _currentUserService.UserId,
                ExperienceId = request.ExperienceId,
                CreatedAt = DateTime.UtcNow
            };
            await _favoriteRepository.AddAsync(favorite);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
