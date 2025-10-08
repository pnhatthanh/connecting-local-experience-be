using System.Linq.Expressions;
using BuildingBlocks.Domain.Models;

namespace BuildingBlocks.Domain.Specifications
{
    public abstract class Specification<T>
        where T : BaseEntity
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public Specification<T> And(Specification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        public Specification<T> Or(Specification<T> other)
        {
            return new OrSpecification<T>(this, other);
        }
    }

}
