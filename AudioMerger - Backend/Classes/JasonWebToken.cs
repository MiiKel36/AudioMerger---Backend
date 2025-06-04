using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AudioMerger___Backend.Classes
{
    public class JasonWebToken
    {
        string secretKey = "fad801847586af2c11e5be56dd153aeb113d1bbe89e32d6fdd6bb437936b6efaf9ba9f0be24a8f3e45899d228cf579e302e14bc49fc31b35e7d518594ba1945b123e349ad6167f71f95cdcc3988addf9e046ed043bb91942edfc69d077111caa8ab2c32c4d3241e9515c30b1f24eb3a8a8adaf0490a481992029810a8a51df613ef033263aeaafd3662b5fb0e2a5766b88eab99210948ac8d0e2edeb6e3691dee1ce42ed96cc622722ff11ef2ce970a76d79ec959e41870daf099397de552b9729243050ff8c2d0005b43554c4d954c0d8b80cea5d8153f40168d2aaa72e2391dbbc804dfc26e41dd5c69cb86ee905a1d91a11b3681438d3a57726dbc6f6fe5c";

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
