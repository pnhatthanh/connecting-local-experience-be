using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using Experience.Domain.Enums;

namespace Experience.Domain.Specifications
{
    public class AdminExperienceFilterSpecification(
        string? searchTerm = null,
        string? status = null,
        Guid? categoryId = null) : Specification<ExperienceEntity>
    {
        private readonly string? _searchTerm = searchTerm;
        private readonly string? _status = status;
        private readonly Guid? _categoryId = categoryId;

        public override Expression<Func<ExperienceEntity, bool>> ToExpression()
        {
            ExperienceStatus? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(_status) && Enum.TryParse<ExperienceStatus>(_status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            return e =>
                (string.IsNullOrWhiteSpace(_searchTerm) ||
                 e.Title.ToLower().Contains(_searchTerm.ToLower()) ||
                 e.Description.ToLower().Contains(_searchTerm.ToLower()) ||
                 (e.Address + " " + e.District + " " + e.City + " " + e.Country).ToLower().Contains(_searchTerm.ToLower())) &&
                (!statusEnum.HasValue || e.Status == statusEnum.Value) &&
                (!_categoryId.HasValue || e.CategoryId == _categoryId.Value);
        }
    }
}
