using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;

namespace Experience.Domain.Specifications
{
    public class ExperienceByHostSpecification(Guid HostId) : Specification<ExperienceEntity>
    {
        public override Expression<Func<ExperienceEntity, bool>> ToExpression()
        {
            return e => e.HostId == HostId;
        }
    }
}
