using BuildingBlocks.Domain.Models;

namespace IAM.Domain.Entities
{
    public class PermissionEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
    }
}
