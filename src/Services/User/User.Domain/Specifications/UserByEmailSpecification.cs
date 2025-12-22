using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class UserByEmailSpecification(string Email) : Specification<UserEntity>
    {
        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            return user => user.Email == Email;
        }
    }
}
