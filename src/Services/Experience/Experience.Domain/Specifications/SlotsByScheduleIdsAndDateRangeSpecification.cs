using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using System.Linq.Expressions;

namespace Experience.Domain.Specifications
{
    public class SlotsByScheduleIdsAndDateRangeSpecification(List<Guid> ScheduleIds, DateOnly StartDate, DateOnly EndDate) : Specification<ExperienceScheduleSlotEntity>
    {
        public override Expression<Func<ExperienceScheduleSlotEntity, bool>> ToExpression()
        {
            return slot => ScheduleIds.Contains(slot.ScheduleId) &&
                          slot.Date >= StartDate &&
                          slot.Date <= EndDate;
        }
    }
}
