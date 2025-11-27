using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Booking.Domain.Entities;

namespace Booking.Domain.Specifications
{
    public class PaymentByIdSpecification(Guid PaymentId) : Specification<PaymentEntity>
    {
        public override Expression<Func<PaymentEntity, bool>> ToExpression()
        {
            return payment => payment.Id == PaymentId;
        }
    }
}
