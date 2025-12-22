using BuildingBlocks.Domain.Specifications;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using System.Linq.Expressions;

namespace IAM.Domain.Specifications;

public class RoleByNameSpecification(AccountRole Role) : Specification<RoleEntity>
{
    public override Expression<Func<RoleEntity, bool>> ToExpression()
    {
        return role => role.Name == Role;
    }
}
