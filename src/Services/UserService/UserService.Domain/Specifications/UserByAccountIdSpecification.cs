using BuildingBlocks.Domain.Models;
using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using UserService.Domain.Entities;

namespace UserService.Domain.Specifications
{
    public class ProfileByAccountIdSpecification : Specification<ProfileEntity>
    {
        private readonly Guid _accountId;

        public ProfileByAccountIdSpecification(Guid accountId)
        {
            _accountId = accountId;
        }

        public override Expression<Func<ProfileEntity, bool>> ToExpression()
        {
            return profile => profile.AccountId == _accountId;
        }
    }
}
