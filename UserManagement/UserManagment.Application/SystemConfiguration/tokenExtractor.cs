using System.IdentityModel.Tokens.Jwt;
using UserManagment.Common.Enum;

namespace UserManagment.Application.SystemConfiguration
{
    public static class TokenExtractor
    {
        public static T GetClaimFromToken<T>(string token, ClaimType claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var claimTypeString = claimType.ToString(); // Convert enum to string
            var result = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == claimTypeString)?.Value;

            if (result == null)
            {
                throw new ApplicationException($"Claim '{claimTypeString}' not found in token.");
            }

            if (typeof(T) == typeof(Guid) && claimType == ClaimType.UserId)
            {
                if (Guid.TryParse(result, out var claimValue))
                {
                    return (T)(object)claimValue;  
                }
                throw new ApplicationException("Invalid UserId format in token.");
            }
            else if (typeof(T) == typeof(string) && claimType == ClaimType.AuthZ)
            {
                return (T)(object)result;
            }
            else
            {
                throw new ApplicationException($"Invalid claim type '{claimTypeString}' or return type '{typeof(T)}'.");
            }
        }
    }
}
