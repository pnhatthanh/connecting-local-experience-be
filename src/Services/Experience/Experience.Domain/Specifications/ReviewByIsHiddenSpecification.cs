using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using Experience.Domain.Entities;

namespace Experience.Domain.Specifications;

public class ReviewByIsHiddenSpecification(bool isHidden) : Specification<ReviewEntity>
{
    public override Expression<Func<ReviewEntity, bool>> ToExpression()
    {
        return review => review.IsHidden == isHidden;
    }
}