using BuildingBlocks.Application.CQRS.Command;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using BuildingBlocks.Domain.Interfaces;
using User.Domain.Repositories;
using User.Domain.Specifications;

namespace User.Application.Handlers.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommandHandler : ICommandHandler<RemoveFavoriteCommand, bool>
    {
        private readonly IUserFavoriteExperienceRepository _favoriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveFavoriteCommandHandler(IUserFavoriteExperienceRepository favoriteRepository,
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _favoriteRepository = favoriteRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var favoriteSpec = new UserFavoriteByUserAndExperienceSpecification(_currentUserService.UserId, request.ExperienceId);
            var favorite = await _favoriteRepository.GetBySpecAsync(favoriteSpec)
                ?? throw new BadRequestException("Favorite not found");

            _favoriteRepository.Delete(favorite);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
    }
}
