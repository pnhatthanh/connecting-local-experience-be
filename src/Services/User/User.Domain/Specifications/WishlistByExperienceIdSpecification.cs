using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class WishlistByExperienceIdSpecification(Guid WishlistId, Guid ExperienceId) : Specification<WishlistExperienceEntity>
    {
        public override Expression<Func<WishlistExperienceEntity, bool>> ToExpression()
        {
            return we => we.WishlistId == WishlistId && we.ExperienceId == ExperienceId;
        }
    }
}
