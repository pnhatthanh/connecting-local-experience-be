using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.Domain.Specifications
{
    public class BookingByHostIdSpecification(Guid HostId, DateOnly Date, TimeSpan? StartTime) : Specification<BookingEntity>
    {
        public override Expression<Func<BookingEntity, bool>> ToExpression()
        {
            return booking => booking.HostId == HostId
                && (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Completed 
                    || booking.Status == BookingStatus.Cancelled)
                && booking.Date == Date
                && (StartTime == null || booking.StartTime == StartTime.Value);
        }
    }
}
