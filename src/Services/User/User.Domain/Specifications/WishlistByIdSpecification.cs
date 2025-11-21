using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class WishlistByIdSpecification(Guid WishlistId) : Specification<UserWishlistEntity>
    {
        public override Expression<Func<UserWishlistEntity, bool>> ToExpression()
        {
            return wishlist => wishlist.Id == WishlistId;
        }
    }
}
