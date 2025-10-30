using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Domain.Specifications
{
    public class HostProfileByVerifyStatusSpecification(VerifyStatus Status) : Specification<HostProfileEntity>
    {
        public override Expression<Func<HostProfileEntity, bool>> ToExpression()
        {
            return host => host.VerifyStatus == Status;
        }
    }
}
