using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Email;
using UserManagment.Common.DTO.EmailDto;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Helpers;
using UserManagment.Common.Messages;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Users
{
    public record ChangePassword(ChangePasswordDTO ChangePasswordDTO) : IRequest<bool>;
    public class ChangePasswordHandler : IRequestHandler<ChangePassword, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<ChangePasswordHandler> _logger;
        private readonly IMediator _mediator;


        public ChangePasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, ILogger<ChangePasswordHandler> logger, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<bool> Handle(ChangePassword request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Authentication.FindUserByIdAsync(request.ChangePasswordDTO.userId);
            if (user == null)
            {
                throw new CustomException(ErrorResponseMessage.User_NotFound);
            }
            // Check if the current password is the same as the current input password
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.ChangePasswordDTO.OldPassword);
            var isEqualPassword = result.Equals(PasswordVerificationResult.Success);

            if (isEqualPassword)
            {
                // send email
                try
                {
                    var newPassword = request.ChangePasswordDTO.NewPassword;
                    var hashedPassword = _passwordHasher.HashPassword(user, newPassword);
                    user.Password = hashedPassword;
                    user.PasswordLastUpdatedDate = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Successfully changed password for user with Id: {Id}", request.ChangePasswordDTO.userId);
                    var emailTemplate = await _unitOfWork.Configuration.GetByKeyAndTypeAsync(Configuration.CHANGE_PASSWORD_KEY, Configuration.EMAIL_TEMPLATE_TYPE);

                    if (emailTemplate == null)
                    {
                        throw new CustomException(ErrorResponseMessage.EmailTemplate_NotFound);
                    }
                    // Replace placeholders in the template
                    var emailBody = emailTemplate.ConfigValue
                        .Replace("{UserFullName}", $"{user.FirstName} {user.LastName}")
                        .Replace("[Your Project Name]", "User Management")
                        .Replace("{WebsiteURL}", "https://yourwebsite.com");

                    // Send email with username and password
                    var emailDto = new EmailDto
                    {
                        ToEmail = user.Email,
                        Subject = MailSubjectMessages.UserChangePassword,
                        Body = emailBody
                    };

                    // Create a send email request
                    var sendEmailRequest = new SendEmailRequest(emailDto);
                    await _mediator.Send(sendEmailRequest, cancellationToken); // Send the email
                    return true;
                }
                catch
                {
                    throw new CustomException(ErrorResponseMessage.ChangePasswordFailed);

                }
            }
            else
            {
                throw new CustomException(ErrorResponseMessage.User_InvalidOldPassword);
            }
            throw new CustomException(ErrorResponseMessage.ChangePasswordFailed);
        }
    }
}