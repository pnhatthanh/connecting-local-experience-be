using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;

namespace Experience.Domain.Specifications
{
    public class ReviewByExperienceIdSpecification(Guid experienceId) : Specification<ReviewEntity>
    {
        public override Expression<Func<ReviewEntity, bool>> ToExpression()
        {
            return review => review.ExperienceId == experienceId;
        }
    }
}