using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagment.Application.Identity
{
    public class TokenAuthentication
    {
        private readonly IConfiguration _configuration;
        private const string TokenSecretKey = "JwtSettings:SecretKey";
        private const string TokenExpirationSecondesKey = "JwtSettings:expire";
        private const string TokenIssuerKey = "JwtSettings:Issuer";
        private const string TokenAudienceKey = "JwtSettings:Audience";
        private readonly string _TokenAudience;
        private readonly string _tokenSecret;
        private readonly int _tokenExpirationSeconds;
        private readonly string _TokenExpirationIssuer;

        public TokenAuthentication(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenSecret = _configuration[TokenSecretKey] ?? "Secret";
            _tokenExpirationSeconds = int.Parse(_configuration[TokenExpirationSecondesKey] ?? "1296000");
            _TokenExpirationIssuer = _configuration[TokenIssuerKey] ?? "Issuer";
            _TokenAudience = _configuration[TokenAudienceKey] ?? "Audience";
        }

        public string GenerateJwtToken(ActiveContext context)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecret));
            var token = new JwtSecurityToken(
                    audience: _TokenAudience,
                    expires: DateTime.Now.AddSeconds(_tokenExpirationSeconds),
                    claims: GetClaims(context),
                    issuer: _TokenExpirationIssuer,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static List<Claim> GetClaims(ActiveContext context)
        {
            List<Claim> list = new List<Claim>();
            AddClaim(nameof(ActiveContext.UserId), context.UserId, list);
            AddClaim(nameof(ActiveContext.UserName), context.UserName, list);
            AddClaim(nameof(ActiveContext.UserType), context.UserType, list);
            // Serialize PoliciesNames list into a comma-separated string
            var policiesNames = string.Join(",", context.PoliciesNames ?? new List<string> { "temp", "temp" });
            AddClaim(nameof(ActiveContext.PoliciesNames), policiesNames, list);
            return list;
        }

        private static void AddClaim(string name, object? value, List<Claim> source)
        {
            if (value is null)
            {
                return;
            }
            source.Add(new Claim(name, value?.ToString() ?? ""));
        }

        public ClaimsPrincipal GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _TokenExpirationIssuer,
                ValidAudience = _TokenAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecret))
            };
        }
    }

}
