using System.Linq.Expressions;
using BuildingBlocks.Domain.Specifications;
using IAM.Domain.Entities;

namespace IAM.Domain.Specifications
{
    public class RefreshTokenByTokenSpecification(Guid userId, string token) : Specification<RefreshTokenEntity>
    {
        private readonly Guid _userId = userId;
        private readonly string _token = token;

        public override Expression<Func<RefreshTokenEntity, bool>> ToExpression()
        {
            return token => token.Token == _token && token.AccountId == _userId;
        }
    }
}
