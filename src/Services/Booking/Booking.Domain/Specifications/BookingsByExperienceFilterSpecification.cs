using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.Domain.Specifications
{
    public class BookingsByExperienceFilterSpecification : Specification<BookingEntity>
    {
        private readonly Guid _experienceId;
        private readonly DateOnly? _date;
        private readonly TimeSpan? _startTime;

        public BookingsByExperienceFilterSpecification(
            Guid experienceId, 
            DateOnly? date = null, 
            TimeSpan? startTime = null)
        {
            _experienceId = experienceId;
            _date = date;
            _startTime = startTime;
        }

        public override Expression<Func<BookingEntity, bool>> ToExpression()
        {
            return booking => 
                booking.ExperienceId == _experienceId &&
                (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Completed 
                    || booking.Status == BookingStatus.Cancelled) &&
                (!_date.HasValue || booking.Date == _date.Value) &&
                (!_startTime.HasValue || booking.StartTime == _startTime.Value);
        }
    }
}
