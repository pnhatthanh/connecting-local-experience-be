using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using System.Linq.Expressions;

namespace Experience.Domain.Specifications
{
    public class ScheduleByExperienceSpecification(Guid ExperienceId) : Specification<ExperienceScheduleEntity>
    {
        public override Expression<Func<ExperienceScheduleEntity, bool>> ToExpression()
        {
            return schedule => schedule.ExperienceId == ExperienceId;
        }
    }
}
