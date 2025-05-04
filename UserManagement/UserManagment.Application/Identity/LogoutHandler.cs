using MediatR;
using Microsoft.AspNetCore.Http;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.Enum;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Identity
{
    public record LogoutCommand : IRequest<bool>;
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenAuthentication _authenticationHandler;

        public LogoutCommandHandler(IUnitOfWork unitOfWork, TokenAuthentication Auth, IHttpContextAccessor HttpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _authenticationHandler = Auth;
            _httpContextAccessor = HttpContextAccessor;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                throw new CustomException(ErrorResponseMessage.Unauthorized);
            }
            var userId = TokenExtractor.GetClaimFromToken<Guid>(token, ClaimType.UserId);

            var accessLogRequest = new AccessLogRequest(
                AccessStatus: AccessStatus.LogOutSuccess,
                userId: userId
            );

            var result = await _unitOfWork.AccessLog.Create(accessLogRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result == null)
            {
                return false;
            }

            return true;
        }
    }

}
