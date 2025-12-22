using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class UserByIdSpecification(Guid UserId) : Specification<UserEntity>
    {

        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            return user => user.Id == UserId;
        }
    }
}
