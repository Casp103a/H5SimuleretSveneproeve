using DrocsidApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace DrocsidApi.Services
{
    public interface ITokenService
    {
        string CreateToken(UserModel user, out Guid id);
        string CreateRefreshToken();
        ClaimsPrincipal ValidateToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;
        private readonly SymmetricSecurityKey key;
        public TokenService(IConfiguration config)
        {
            this.config = config;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SigningKey"]));
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        public string CreateToken(UserModel user, out Guid id)
        {
            id = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.GivenName, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Sid, id.ToString())
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = creds,
                Issuer = config["JWT:Issuer"],
                Audience = config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SigningKey"]));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = config["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = config["JWT:Audience"],
                ValidateIssuerSigningKey = true,
                ClockSkew = new TimeSpan(0),
            };

            ClaimsPrincipal principal;
            try
            {
                principal = new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
            }
            catch (SecurityTokenExpiredException e)
            {
                throw;
            }

            return principal;
        }
    }
}
