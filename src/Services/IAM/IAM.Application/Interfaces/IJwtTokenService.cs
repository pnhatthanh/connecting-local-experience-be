using IAM.Domain.Entities;

namespace IAM.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(AccountEntity account, IEnumerable<string> permissions);
        string GenerateRefreshToken();
    }
}