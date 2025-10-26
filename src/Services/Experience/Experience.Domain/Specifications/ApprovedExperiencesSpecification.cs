using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using Experience.Domain.Enums;

namespace Experience.Domain.Specifications
{
    public class ApprovedExperiencesSpecification : Specification<ExperienceEntity>
    {
        public override Expression<Func<ExperienceEntity, bool>> ToExpression()
        {
            return e => e.Status == ExperienceStatus.Approved;
        }
    }
}
