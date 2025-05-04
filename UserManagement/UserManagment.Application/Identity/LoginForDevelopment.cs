using UserManagment.Common.DTO.UserRecords;
using UserManagment.Application.Abstractions.DataAbstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static UserManagment.Application.Identity.LoginForDevelopmentHandler;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Identity
{
    public class LoginForDevelopmentHandler : IRequestHandler<LoginForDev, LoginResult>
    {
        public record LoginForDev(string EmailAddress, string Password) : IRequest<LoginResult>;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly TokenAuthentication _authenticationHandler;

        public LoginForDevelopmentHandler(IUnitOfWork unitOfWork, TokenAuthentication authenticationHandler, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _authenticationHandler = authenticationHandler;
        }

        public async Task<LoginResult> Handle(LoginForDev request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Authentication.GetByEmailAsync(
                new Login(request.EmailAddress, request.Password), cancellationToken);
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            var isEqualPassword = result.Equals(PasswordVerificationResult.Success);
            var UserRoles = await _unitOfWork.GroupRole.GetDistinctRoleIdsByUserAsync(user.Id, cancellationToken);
            var RoleScreenActionids = await _unitOfWork.RoleScreenAction.GetDistinctScreenIdsByRoleIdsAsync(UserRoles, cancellationToken);
            var policiesNames = await _unitOfWork.ScreenAction.GetPoliciesNamebyScreenIds(RoleScreenActionids, cancellationToken);
            if (isEqualPassword)
            {
                return new LoginResult
                {
                    IsSuccess = true,
                    Token = _authenticationHandler.GenerateJwtToken(new ActiveContext
                    {
                        UserId = user.Id,
                        UserName = user.FirstName + " " + user.LastName,
                        UserType = user.UserType,
                        PoliciesNames = policiesNames
                    }),
                    Id = user.Id,

                };
            }
            else
            {
                return null;
            }
        }

    }

}
