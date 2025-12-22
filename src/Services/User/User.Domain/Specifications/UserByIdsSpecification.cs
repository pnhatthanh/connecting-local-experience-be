using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Domain.Specifications;

public class UserByIdsSpecification : Specification<UserEntity>
{
    private readonly List<Guid> _userIds;

    public UserByIdsSpecification(List<Guid> userIds)
    {
        _userIds = userIds;
    }

    public override Expression<Func<UserEntity, bool>> ToExpression()
    {
        return user => _userIds.Contains(user.Id) && user.Role == UserRole.Host;
    }
}
