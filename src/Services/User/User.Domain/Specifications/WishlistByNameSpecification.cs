using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using User.Domain.Entities;

namespace User.Domain.Specifications
{
    public class WishlistByNameSpecification(Guid UserId, string Name) : Specification<UserWishlistEntity>
    {
        public override Expression<Func<UserWishlistEntity, bool>> ToExpression()
        {
            return wishlist => wishlist.UserId == UserId && wishlist.Name == Name;
        }
    }
}
