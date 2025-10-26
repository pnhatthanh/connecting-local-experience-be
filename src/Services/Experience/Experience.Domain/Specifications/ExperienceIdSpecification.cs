using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;

namespace Experience.Domain.Specifications;

public class ExperienceIdSpecification(Guid ExperienceId) : Specification<ExperienceEntity>
{
    public override Expression<Func<ExperienceEntity, bool>> ToExpression()
    {
        return experience => experience.Id == ExperienceId;
    }
}