using Microsoft.AspNetCore.Http;
using UserManagment.Common.Enum;

namespace UserManagment.Application.Identity
{
    public class ActiveContext
    {
        public Guid UserId { get; set; }
        public object GrouIds { get; internal set; }
        public UserType UserType { get; internal set; }
        public string UserName { get; internal set; }
        public List<string> PoliciesNames { get; internal set; }


        public static ActiveContext GetActiveContext(IServiceProvider serviceProvider)
        {
            var httpContextAccessor = serviceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var activeContext = httpContextAccessor?.HttpContext?.Items["ActiveContext"] as ActiveContext;
            return activeContext;
        }
    }

}
