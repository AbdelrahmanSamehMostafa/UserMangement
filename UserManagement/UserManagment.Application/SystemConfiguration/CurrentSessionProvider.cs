using Microsoft.AspNetCore.Http;
using UserManagment.Common.Enum;

namespace UserManagment.Application.SystemConfiguration
{
    public interface ICurrentSessionProvider
    {
        Guid? GetUserId();
    }

    public class CurrentSessionProvider : ICurrentSessionProvider
    {
        private readonly Guid? _currentUserId;

        public CurrentSessionProvider(IHttpContextAccessor accessor)
        {
            if (!accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                return;
            }

            var token = tokenHeader.ToString().Replace("Bearer ", "");
            var userId = TokenExtractor.GetClaimFromToken<Guid>(token, ClaimType.UserId);

            _currentUserId = userId != Guid.Empty ? userId : null;
        }

        public Guid? GetUserId() => _currentUserId;
    }

}