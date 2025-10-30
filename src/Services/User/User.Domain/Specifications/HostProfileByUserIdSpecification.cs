using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class HostProfileByUserIdSpecification(Guid UserId) : Specification<HostProfileEntity>
    {
        public override Expression<Func<HostProfileEntity, bool>> ToExpression()
        {
            return host => host.UserId == UserId;
        }
    }
}
