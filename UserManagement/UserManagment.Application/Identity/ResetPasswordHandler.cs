using System.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Email;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Application.Users;
using UserManagment.Common.DTO.EmailDto;
using UserManagment.Common.Helpers;
using UserManagment.Common.Messages;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Identity
{
    public record ResetPassword(string Email) : IRequest<bool>;


    public class ResetPasswordHandler : IRequestHandler<ResetPassword, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMediator _mediator;
        private readonly UserCredentialGenerator _userCredentialGenerator;


        public ResetPasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
            _userCredentialGenerator = new UserCredentialGenerator();
        }

        public async Task<bool> Handle(ResetPassword request, CancellationToken cancellationToken)
        {
            // Validate the email domain
            var domains = await _mediator.Send(new DomainFormat(), cancellationToken);
            var validDomains = domains.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim()).ToList(); //all valid domains

            if (!validDomains.Any(d => request.Email.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
            {
                throw new CustomException(ErrorResponseMessage.User_MailDomain);
            }

            var user = await _unitOfWork.Authentication.GetUserByMailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                throw new CustomException(ErrorResponseMessage.User_NotFound);
            }
            try
            {
                var newPassword = _userCredentialGenerator.GenerateRandomPassword();
                var hashedPassword = _passwordHasher.HashPassword(user, newPassword);
                user.Password = hashedPassword;
                user.PasswordLastUpdatedDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var emailTemplate = await _unitOfWork.Configuration.GetByKeyAndTypeAsync(Configuration.RESET_PASSWORD_KEY, Configuration.EMAIL_TEMPLATE_TYPE);
                if (emailTemplate == null)
                {
                    throw new CustomException(ErrorResponseMessage.EmailTemplate_NotFound);
                }
                // Replace placeholders in the template
                var emailBody = emailTemplate.ConfigValue
                    .Replace("{UserFullName}", $"{user.FirstName} {user.LastName}")
                    .Replace("{UserPassword}", newPassword)
                    .Replace("[Your Project Name]", "User Management")
                    .Replace("{WebsiteURL}", "https://yourwebsite.com");

                // Send email with username and password
                var emailDto = new EmailDto
                {
                    ToEmail = user.Email,
                    Subject = MailSubjectMessages.UserResetPassword,
                    Body = emailBody
                };

                // Create a send email request
                var sendEmailRequest = new SendEmailRequest(emailDto);
                await _mediator.Send(sendEmailRequest, cancellationToken); // Send the email
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}