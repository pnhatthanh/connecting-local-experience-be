using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using Experience.Domain.Enums;

namespace Experience.Domain.Specifications
{
    public class ExperienceFilterSpecification(Guid? CategoryId = null, ExperienceStatus? Status = null) : Specification<ExperienceEntity>
    {
        public override Expression<Func<ExperienceEntity, bool>> ToExpression()
        {
            return e => (!CategoryId.HasValue || e.CategoryId == CategoryId.Value) &&
                       (!Status.HasValue || e.Status == Status.Value);
        }
    }
}
