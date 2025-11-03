using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;
using User.Domain.Enums;

namespace User.Domain.Specifications
{
    public class HostFilterSpecification : Specification<UserEntity>
    {
        private readonly string? _searchTerm;
        private readonly string? _verifyStatus;

        public HostFilterSpecification(string? searchTerm, string? verifyStatus)
        {
            _searchTerm = searchTerm;
            _verifyStatus = verifyStatus;
        }

        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            VerifyStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(_verifyStatus) && Enum.TryParse<VerifyStatus>(_verifyStatus, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            return u => 
                u.HostProfile != null &&
                (string.IsNullOrWhiteSpace(_searchTerm) || 
                 u.FullName.ToLower().Contains(_searchTerm.ToLower()) ||
                 u.Email.ToLower().Contains(_searchTerm.ToLower()) ||
                 (u.HostProfile.Location != null && u.HostProfile.Location.ToLower().Contains(_searchTerm.ToLower()))) &&
                (!statusEnum.HasValue || u.HostProfile.VerifyStatus == statusEnum.Value);
        }
    }
}
