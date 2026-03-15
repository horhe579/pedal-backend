using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pedal.Models;

namespace Pedal.Services
{
    public class TokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));

            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
                throw new ArgumentException("JWT SecretKey cannot be null or empty", nameof(jwtSettings));
            if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
                throw new ArgumentException("JWT Issuer cannot be null or empty", nameof(jwtSettings));
            if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
                throw new ArgumentException("JWT Audience cannot be null or empty", nameof(jwtSettings));
            if (_jwtSettings.ExpirationMinutes <= 0)
                throw new ArgumentException("JWT ExpirationMinutes must be positive", nameof(jwtSettings));

            var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            if (keyBytes.Length < 32)
                throw new ArgumentException("JWT SecretKey must be at least 32 characters (256 bits)", nameof(jwtSettings));
        }

        public string GenerateToken(string carId, string email)
        {
            if (string.IsNullOrWhiteSpace(carId))
                throw new ArgumentException("CarId cannot be null or empty", nameof(carId));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("carId", carId),
                new Claim("email", email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
