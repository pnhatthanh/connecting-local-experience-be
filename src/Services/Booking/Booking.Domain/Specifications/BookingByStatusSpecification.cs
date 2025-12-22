using System.Linq.Expressions;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using BuildingBlocks.Domain.Specifications;

namespace Booking.Domain.Specifications
{
    public class BookingByStatusSpecification(BookingStatus status) : Specification<BookingEntity>
    {
        private readonly BookingStatus _status = status;

        public override Expression<Func<BookingEntity, bool>> ToExpression()
        {
            return booking => booking.Status != _status;
        }
    }
}