using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;
using Experience.Domain.Enums;

namespace Experience.Domain.Specifications
{
    public class ExperienceFilterSpecification(
        string? SearchTerm = null,
        Guid? HostId = null,
        List<Guid>? CategoryIds = null,
        int? MinDuration = null,
        int? MaxDuration = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        List<string>? Languages = null,
        string? Status = null
    ) : Specification<ExperienceEntity>
    {
        private readonly ExperienceStatus? _parsedStatus =
            (!string.IsNullOrEmpty(Status) &&
             Enum.TryParse(Status, true, out ExperienceStatus temp))
                ? temp
                : null;
        public override Expression<Func<ExperienceEntity, bool>> ToExpression()
        {
            return e =>
                (string.IsNullOrEmpty(SearchTerm) ||
                 e.Title.ToLower().Contains(SearchTerm.ToLower()) ||
                 e.Description.ToLower().Contains(SearchTerm.ToLower()) ||
                 (e.Address + " " + e.District + " " + e.City + " " + e.Country)
                    .ToLower()
                    .Contains(SearchTerm.ToLower())) &&

                (!HostId.HasValue || e.HostId == HostId.Value) &&
                (CategoryIds == null || CategoryIds.Count == 0 || CategoryIds.Contains(e.CategoryId)) &&
                (!MinDuration.HasValue || e.Duration >= MinDuration.Value) &&
                (!MaxDuration.HasValue || e.Duration <= MaxDuration.Value) &&
                (!MinPrice.HasValue || e.AdultPrice >= MinPrice.Value) &&
                (!MaxPrice.HasValue || e.AdultPrice <= MaxPrice.Value) &&
                (Languages == null || Languages.Count == 0 || Languages.Contains(e.Language)) &&
                (_parsedStatus == null || e.Status == _parsedStatus.Value);
        }
    }
}
