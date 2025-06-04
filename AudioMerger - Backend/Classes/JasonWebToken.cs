using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AudioMerger___Backend.Classes
{
    public class JasonWebToken
    {

        public string GenerateJwtToken(string username, int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey); // Same secretKey as in GenerateJwtToken

            var validationParameters = new TokenValidationParameters
            {
                // Since GenerateJwtToken doesn't set Issuer/Audience, we skip validation
                ValidateIssuer = false,
                ValidateAudience = false,

                // Critical validations (must match generation)
                ValidateLifetime = true, // Check token expiration (exp)
                ValidateIssuerSigningKey = true, // Verify signature
                IssuerSigningKey = new SymmetricSecurityKey(key), // Same key as generation
                ClockSkew = TimeSpan.Zero // Strict expiry validation
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                // Log the error if needed (e.g., "Invalid token: {ex.Message}")
                return null;
            }
        }
    }
}
