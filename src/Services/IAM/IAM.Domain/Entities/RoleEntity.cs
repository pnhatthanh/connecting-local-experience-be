using BuildingBlocks.Domain.Models;
using IAM.Domain.Enums;

namespace IAM.Domain.Entities
{
    public class RoleEntity : BaseEntity
    {
        public AccountRole Name { get; set; } 
        public string? Description { get; set; }
        public virtual ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
        public virtual ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
    }
}