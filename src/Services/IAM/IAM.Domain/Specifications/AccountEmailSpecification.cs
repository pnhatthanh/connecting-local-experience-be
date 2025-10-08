using BuildingBlocks.Domain.Specifications;
using IAM.Domain.Entities;
using System.Linq.Expressions;

namespace IAM.Domain.Specifications
{
    public class AccountEmailSpecification(string Email) : Specification<AccountEntity>
    {
        public override Expression<Func<AccountEntity, bool>> ToExpression()
        {
            return account => account.Email == Email;
        }
    }
}