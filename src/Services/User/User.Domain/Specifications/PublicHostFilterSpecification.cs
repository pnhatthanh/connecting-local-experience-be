using BuildingBlocks.Domain.Specifications;
using System.Linq.Expressions;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class PublicHostFilterSpecification : Specification<UserEntity>
    {
        private readonly string? _searchTerm;
        private readonly string? _location;
        private readonly string[]? _spokenLanguages;
        private readonly string[]? _topicsOfInterest;

        public PublicHostFilterSpecification(
            string? searchTerm, 
            string? location,
            string[]? spokenLanguages,
            string[]? topicsOfInterest)
        {
            _searchTerm = searchTerm;
            _location = location;
            _spokenLanguages = spokenLanguages;
            _topicsOfInterest = topicsOfInterest;
        }

        public override Expression<Func<UserEntity, bool>> ToExpression()
        {
            return user =>
                // Must have host profile and be verified
                user.HostProfile != null &&
                user.HostProfile.IsVerified &&
                // Search by name or location
                (string.IsNullOrEmpty(_searchTerm) ||
                    user.FullName.ToLower().Contains(_searchTerm.ToLower()) ||
                    (user.HostProfile.Location != null && user.HostProfile.Location.ToLower().Contains(_searchTerm.ToLower()))) &&
                // Filter by location
                (string.IsNullOrEmpty(_location) ||
                    (user.HostProfile.Location != null && user.HostProfile.Location.ToLower().Contains(_location.ToLower()))) &&
                // Filter by spoken languages
                (_spokenLanguages == null || _spokenLanguages.Length == 0 ||
                    _spokenLanguages.Any(lang => user.HostProfile.SpokenLanguages.Contains(lang))) &&
                // Filter by topics of interest
                (_topicsOfInterest == null || _topicsOfInterest.Length == 0 ||
                    _topicsOfInterest.Any(topic => user.HostProfile.TopicsOfInterest.Contains(topic)));
        }
    }
}
