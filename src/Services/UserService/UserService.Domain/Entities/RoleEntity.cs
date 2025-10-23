using BuildingBlocks.Domain.Models;
using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    /// <summary>
    /// Entity đại diện cho bảng tbl_role
    /// </summary>
    public class RoleEntity : BaseEntity
    {
        public AccountRole Name { get; set; }
        public string? Description { get; set; }
    }
}
