using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Domain.Specifications
{
    public class UserFilterSpecification : Specification<UserEntity>
    {
        private readonly string? _searchTerm;
        private readonly string? _role;

        public UserFilterSpecification(string? searchTerm, string? role)
        {
            _searchTerm = searchTerm;
            _role = role;
        }

        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            return u => 
                (string.IsNullOrWhiteSpace(_searchTerm) || 
                 u.FullName.ToLower().Contains(_searchTerm.ToLower()) ||
                 u.Email.ToLower().Contains(_searchTerm.ToLower())) &&
                (string.IsNullOrWhiteSpace(_role) || 
                 u.Role == Enum.Parse<UserRole>(_role, true));
        }
    }
}
