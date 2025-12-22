using Booking.Domain.Entities;
using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;

namespace Booking.Domain.Specifications
{
    public class PaymentByTransactionIdSpecification(string transactionId) : Specification<PaymentEntity>
    {
        public override Expression<Func<PaymentEntity, bool>> ToExpression()
        {
            return payment => payment.TransactionId == transactionId;
        }
    }
}
