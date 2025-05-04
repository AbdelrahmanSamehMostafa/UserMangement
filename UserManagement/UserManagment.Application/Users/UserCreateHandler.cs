using System.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.Email;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.EmailDto;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Helpers;
using UserManagment.Common.Messages;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Users
{
    // Request for getting list of users
    public record UserRequestDTO(
        string FirstName,
        string LastName,
        string Email,
        string MobileNumber,
        string Location,
        DateTime DateOfBirth, List<Guid> GroupIds) : IRequest<UserResultDto>;

    public class UserCreateHandler : IRequestHandler<UserRequestDTO, UserResultDto>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMediator _mediator;
        private readonly UserCredentialGenerator _userCredentialGenerator;

        public UserCreateHandler(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher,
        IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
            _userCredentialGenerator = new UserCredentialGenerator();
        }

        public async Task<UserResultDto> Handle(UserRequestDTO request, CancellationToken cancellationToken)
        {
            // Validate the email domain
            var domains = await _mediator.Send(new DomainFormat(), cancellationToken);
            var validDomains = domains.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim()).ToList(); //all valid domains

            if (!validDomains.Any(d => request.Email.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
            {
                throw new CustomException(ErrorResponseMessage.User_MailDomain);
            }
            if (!request.GroupIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.NoGroups);
            }

            var userDto = new UserDetailsDTO
            {
                Id = Guid.Empty,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                MobileNumber = request.MobileNumber,
                Location = request.Location,
                DateOfBirth = request.DateOfBirth
            };
            bool isFound = await _unitOfWork.Authentication.IsFound(userDto.Email, userDto.Id);
            if (isFound)
            {
                throw new CustomException(ErrorResponseMessage.isRepeated);
            }

            // Generate a username and password
            var (username, password) = _userCredentialGenerator.GenerateRandomUserCredentials();

            var newUser = request.ToUser();
            Console.WriteLine($"password: {password}", password);

            newUser.UserName = username;
            var hashedPassword = _passwordHasher.HashPassword(newUser, password);
            newUser.Password = hashedPassword;
            newUser.PasswordLastUpdatedDate = DateTime.UtcNow;

            var result = await _unitOfWork.Authentication.Create(newUser);
            await _unitOfWork.GroupUser.addGroupsUserAsync(newUser.Id, request.GroupIds, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Fetch the email template
            var emailTemplate = await _unitOfWork.Configuration.GetByKeyAndTypeAsync(Configuration.WELCOME_EMAIL_KEY, Configuration.EMAIL_TEMPLATE_TYPE);
            if (emailTemplate == null)
            {
                throw new CustomException(ErrorResponseMessage.EmailTemplate_NotFound);
            }

            // Replace placeholders in the template
            var emailBody = emailTemplate.ConfigValue
                .Replace("{UserFullName}", $"{newUser.FirstName} {newUser.LastName}")
                .Replace("{UserEmail}", newUser.Email)
                .Replace("{UserUsername}", newUser.UserName)
                .Replace("{UserPassword}", password)
                .Replace("{Your Project Name}", "User Management")
                .Replace("{WebsiteURL}", "https://yourwebsite.com");

            // Send email with username and password
            var emailDto = new EmailDto
            {
                ToEmail = newUser.Email,
                Subject = MailSubjectMessages.UserCreation,
                Body = emailBody
            };

            // Create a send email request
            var sendEmailRequest = new SendEmailRequest(emailDto);
            await _mediator.Send(sendEmailRequest, cancellationToken); // Send the email

            var userResultDto = UserMapping.ToUserResult(result);
            return userResultDto;
        }
    }
}