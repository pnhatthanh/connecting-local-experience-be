using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Domain.Specifications
{
    public class UserFilterSpecification(string? searchTerm, string? role, string? status) : Specification<UserEntity>
    {
        private readonly string? _searchTerm = searchTerm;
        private readonly string? _role = role;
        private readonly string? _status = status;

        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            UserRole? roleEnum = null;
            if (!string.IsNullOrWhiteSpace(_role) && Enum.TryParse<UserRole>(_role, true, out var parsedRole))
                roleEnum = parsedRole;
            UserStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(_status) && Enum.TryParse<UserStatus>(_status, true, out var parsedStatus))
                statusEnum = parsedStatus;

            return u => 
                (string.IsNullOrWhiteSpace(_searchTerm) || 
                 u.FullName.ToLower().Contains(_searchTerm.ToLower()) ||
                 u.Email.ToLower().Contains(_searchTerm.ToLower())) &&
                (!roleEnum.HasValue || u.Role == roleEnum.Value) &&
                (!statusEnum.HasValue || u.Status == statusEnum.Value);
        }
    }
}
