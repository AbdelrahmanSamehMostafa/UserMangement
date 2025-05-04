using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.Exceptions;
using UserManagment.Common.DTO.UserRecords;
using UserManagment.Common.Helpers;
using UserManagment.Common.Messages;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Identity
{
    public record Login(string EmailAddress, string Password) : IRequest<LoginResult>;


    public class LoginHandler : IRequestHandler<Login, LoginResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly TokenAuthentication _authenticationHandler;


        public LoginHandler(IUnitOfWork unitOfWork, TokenAuthentication authenticationHandler, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _authenticationHandler = authenticationHandler;
        }

        public async Task<LoginResult> Handle(Login request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Authentication.GetByEmailAsync(request, cancellationToken);
            var maxTrialsConfig = _unitOfWork.Configuration.GetByKeyAsync(Configuration.MaxTrial_Key_MaxTrial).Result.ConfigValue;

            bool isOk = checkUserValidation(user);

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            var isEqualPassword = result.Equals(PasswordVerificationResult.Success);

            if (isEqualPassword)
            {
                isOk = await checkPasswordExpiration(user.PasswordLastUpdatedDate ?? user.InsertedDate);

                if (!isOk)
                {
                    //password expired
                    await _unitOfWork.AccessLog.Create(new AccessLogRequest
                   (
                         Common.Enum.AccessStatus.LoginPasswordExpired,
                          user.Id
                    ));

                    user.UserTrails++;
                    await _unitOfWork.Authentication.Update(user);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    throw new CustomException(ErrorResponseMessage.Login_PasswordExpired);
                }

                await _unitOfWork.AccessLog.Create(new AccessLogRequest
                 (
                       Common.Enum.AccessStatus.LoginSuccess,
                        user.Id
                  ));
                user.UserTrails = 0;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var UserRoles = await _unitOfWork.GroupRole.GetDistinctRoleIdsByUserAsync(user.Id, cancellationToken);
                var RoleScreenActionids= await _unitOfWork.RoleScreenAction.GetDistinctScreenIdsByRoleIdsAsync(UserRoles, cancellationToken);
                var policiesNames= await _unitOfWork.ScreenAction.GetPoliciesNamebyScreenIds(RoleScreenActionids, cancellationToken);
                return new LoginResult
                {
                    IsSuccess = true,
                    Token = _authenticationHandler.GenerateJwtToken(new ActiveContext
                    {
                        UserId = user.Id,
                        UserName = user.FirstName + " " + user.LastName,
                        UserType = user.UserType,
                        PoliciesNames=policiesNames,
                    }),
                    Id = user.Id,


                };
            }
            else
            {

                if (user.UserTrails >= int.Parse(maxTrialsConfig) || user.IsLocked)
                {
                    user.IsLocked = true;
                    await _unitOfWork.AccessLog.Create(new AccessLogRequest
                    (
                          Common.Enum.AccessStatus.LoginAccountLocked,
                           user.Id
                    ));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    throw new CustomException(ErrorResponseMessage.User_Locked);
                }
                //check maxTrail
                await _unitOfWork.AccessLog.Create(new AccessLogRequest
                 (
                       Common.Enum.AccessStatus.LoginFailed,
                        user.Id
                  ));

                user.UserTrails++;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new CustomException(ErrorResponseMessage.User_Invalidcredentials);
            }

        }

        private bool checkUserValidation(User? user)
        {
            if (user == null)
            {
                throw new CustomException(ErrorResponseMessage.User_Invalidcredentials);
            }
            if (user.IsLocked)
            {
                throw new CustomException(ErrorResponseMessage.Login_AccountLocked);
            }
            if (user.IsActive)
            {
                throw new CustomException(ErrorResponseMessage.Login_AccountNotActive);
            }
            return true;
        }

        private async Task<bool> checkPasswordExpiration(DateTime? passwordLastUpdatedDate)
        {
            bool isOk = false;
            //check passwordExpiration
            var MaxDurationInMonth = await _unitOfWork.Configuration.GetByKeyAsync(Configuration.PasswordExpirationPeriod_Key);
            if (!string.IsNullOrWhiteSpace(MaxDurationInMonth?.ConfigValue))
            {
                var durationInMonth = int.Parse(MaxDurationInMonth?.ConfigValue);
                //getdateDiffrent
                DateTime start = passwordLastUpdatedDate.Value.AddMonths(durationInMonth);
                DateTime end = DateTime.Now.AddMonths(durationInMonth);

                // Calculate the difference between 'end' and 'start' DateTime objects
                var difference = (end - start).Days; // Create TimeSpan object representing the duration
                isOk = difference >= 0;
            }
            return isOk;
        }
    }

    public record TokenResult(string Token);
}
