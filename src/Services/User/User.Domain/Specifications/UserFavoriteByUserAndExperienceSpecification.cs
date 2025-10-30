using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class UserFavoriteByUserAndExperienceSpecification(Guid UserId, Guid ExperienceId) 
        : Specification<UserFavoriteExperienceEntity>
    {
        public override Expression<Func<UserFavoriteExperienceEntity, bool>> ToExpression()
        {
            return f => f.UserId == UserId &&
                f.ExperienceId == ExperienceId;
        }
    }
}
