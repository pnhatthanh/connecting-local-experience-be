using IAM.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IAM.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using IAM.Domain.Entities;

namespace IAM.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSetting _jwtSetting;

        public JwtTokenService(IOptions<JwtSetting> jwtSetting)
        {
            _jwtSetting = jwtSetting.Value;
        }
        public string GenerateAccessToken(AccountEntity account, IEnumerable<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email.ToString()),
                new Claim("EmailConfirmed", account.IsEmailConfirmed.ToString()),
                new Claim(ClaimTypes.Role, account.Role.Name.ToString())
            };
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpirationMinutes),
                Issuer = _jwtSetting.Issuer,
                Audience = _jwtSetting.Audience,
                SigningCredentials = GetSigningCredentials()
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.ASCII.GetBytes(_jwtSetting.SecretKey);
            var securityKey = new SymmetricSecurityKey(key);
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}