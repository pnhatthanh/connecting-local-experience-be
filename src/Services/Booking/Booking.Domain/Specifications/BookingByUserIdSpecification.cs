using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Booking.Domain.Entities;
using Booking.Domain.Enums;

namespace Booking.Domain.Specifications
{
    public class BookingByUserIdSpecification(Guid UserId) : Specification<BookingEntity>
    {
        public override Expression<Func<BookingEntity, bool>> ToExpression()
        {
            return booking => booking.UserId == UserId 
                && booking.Status != BookingStatus.Pending;
        }
    }
}
