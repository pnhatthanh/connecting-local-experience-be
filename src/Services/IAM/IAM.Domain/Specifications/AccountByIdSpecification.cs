using BuildingBlocks.Domain.Specifications;
using IAM.Domain.Entities;
using System.Linq.Expressions;

namespace IAM.Domain.Specifications
{
    public class AccountByIdSpecification(Guid AccountId) : Specification<AccountEntity>
    {
        public override Expression<Func<AccountEntity, bool>> ToExpression()
        {
            return account => account.Id == AccountId;
        }
    }
}
