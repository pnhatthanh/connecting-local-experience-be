using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using IAM.Domain.Entities;

namespace IAM.Domain.Specifications
{
    public class RolePermissionByRoleIdSpecification : Specification<RolePermissionEntity>
    {
        private readonly Guid _roleId;

        public RolePermissionByRoleIdSpecification(Guid roleId)
        {
            _roleId = roleId;
        }

        public override Expression<Func<RolePermissionEntity, bool>> ToExpression()
        {
            return rp => rp.RoleId == _roleId && rp.Licensed;
        }
    }
}
