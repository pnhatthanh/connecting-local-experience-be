using BuildingBlocks.Domain.Models;

namespace IAM.Domain.Entities
{
    public class RolePermissionEntity : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public bool Licensed { get; set; } = true;
        public virtual RoleEntity Role { get; set; } = null!;
        public virtual PermissionEntity Permission { get; set; } = null!;
    }
}