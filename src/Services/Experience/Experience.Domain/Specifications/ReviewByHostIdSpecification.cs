using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;

namespace Experience.Domain.Specifications
{
    public class ReviewByHostIdSpecification(Guid hostId) : Specification<ReviewEntity>
    {
        public override Expression<Func<ReviewEntity, bool>> ToExpression()
        {
            return review => review.HostId == hostId;
        }
    }
}
