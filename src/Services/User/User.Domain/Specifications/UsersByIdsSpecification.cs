using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class UsersByIdsSpecification : Specification<UserEntity>
    {
        private readonly List<Guid> _userIds;

        public UsersByIdsSpecification(List<Guid> userIds)
        {
            _userIds = userIds;
        }

        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            return user => _userIds.Contains(user.Id);
        }
    }
}
