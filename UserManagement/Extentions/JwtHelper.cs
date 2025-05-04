using System.IdentityModel.Tokens.Jwt;

namespace com.gbg.modules.utility.Extentions
{
    public static class JwtHelper
    {
        public static (string UserId, string UserName, string UserType, List<string> PoliciesNames) DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            var userType = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;
            var policiesNamesString = jwtToken.Claims.FirstOrDefault(c => c.Type == "PoliciesNames")?.Value;

            // Split the PoliciesNames string into a List<string>
            var policiesNames = policiesNamesString?.Split(',').ToList() ?? new List<string>();

            return (UserId: userId, UserName: userName, UserType: userType, PoliciesNames: policiesNames);
        }
    }
}