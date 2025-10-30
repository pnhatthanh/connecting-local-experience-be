using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class UserFavoriteByUserIdSpecification(Guid UserId) : Specification<UserFavoriteExperienceEntity>
    {
        public override Expression<Func<UserFavoriteExperienceEntity, bool>> ToExpression()
        {
            return fav => fav.UserId == UserId;
        }
    }
}
