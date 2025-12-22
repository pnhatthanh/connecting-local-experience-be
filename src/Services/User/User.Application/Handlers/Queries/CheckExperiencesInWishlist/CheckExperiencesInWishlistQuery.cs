using BuildingBlocks.Application.CQRS.Query;

namespace User.Application.Handlers.Queries.CheckExperiencesInWishlist
{
    public record CheckExperiencesInWishlistQuery(Guid UserId, List<Guid> ExperienceIds) : IQuery<List<Guid>>;
}
