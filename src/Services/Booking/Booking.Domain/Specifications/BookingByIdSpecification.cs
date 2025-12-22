using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Booking.Domain.Entities;

namespace Booking.Domain.Specifications
{
    public class BookingByIdSpecification(Guid BookingId) : Specification<BookingEntity>
    {
        public override Expression<Func<BookingEntity, bool>> ToExpression()
        {
            return booking => booking.Id == BookingId;
        }
    }
}
