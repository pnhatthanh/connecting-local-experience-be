using BuildingBlocks.Domain.Specifications;
using IAM.Domain.Entities;
using System.Linq.Expressions;

namespace IAM.Domain.Specifications
{
    public class RefreshTokenSpecification(string Token) : Specification<RefreshTokenEntity>
    {
        public override Expression<Func<RefreshTokenEntity, bool>> ToExpression()
        {
            return refreshToken => refreshToken.Token == Token;
        }
    }
}
