using BuildingBlocks.Application.CQRS.Query;
using User.Domain.Repositories;

namespace User.Application.Handlers.Queries.CheckExperiencesInWishlist
{
    public class CheckExperiencesInWishlistQueryHandler : IQueryHandler<CheckExperiencesInWishlistQuery, List<Guid>>
    {
        private readonly IWishlistExperienceRepository _wishlistExperienceRepository;

        public CheckExperiencesInWishlistQueryHandler(
            IWishlistExperienceRepository wishlistExperienceRepository)
        {
            _wishlistExperienceRepository = wishlistExperienceRepository;
        }

        public async Task<List<Guid>> Handle(CheckExperiencesInWishlistQuery request, CancellationToken cancellationToken)
        {
            if (!request.ExperienceIds.Any())
                return new List<Guid>();

            var favoriteExperienceIds = await _wishlistExperienceRepository.GetFavoriteExperienceIdsByUserIdAsync(request.UserId, request.ExperienceIds);
            return favoriteExperienceIds;
        }
    }
}
