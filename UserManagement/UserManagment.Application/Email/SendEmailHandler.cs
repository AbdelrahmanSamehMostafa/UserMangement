using System.Net.Mail;
using MediatR;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.EmailDto;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Email
{
    public record SendEmailRequest(EmailDto MailRequest) : IRequest<Unit>;

    public class SendEmailHandler : IRequestHandler<SendEmailRequest, Unit>
    {
        private readonly IConfigurationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SendEmailHandler> _logger;

        public SendEmailHandler(IConfigurationRepository repository, IUnitOfWork unitOfWork, ILogger<SendEmailHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendEmailRequest request, CancellationToken cancellationToken)
        {
            var mailRequest = request.MailRequest;
            _logger.LogInformation("In SendEmailHandler.Handle method with mail request: {@MailRequest}", mailRequest);

            if (mailRequest == null) throw new CustomException(ErrorResponseMessage.SendEmailRequest);

            MailMessage msg = new MailMessage();

            List<Configuration> list = await _repository.GetByTypeAsync(Configuration.EMAIL_TYPE);

            _logger.LogInformation("Total configurations retrieved: {Count}", list.Count);

            // Extract and log configurations
            string userName = list.FirstOrDefault(x => x.ConfigKey == Configuration.SMPTUSERNAME)?.ConfigValue;
            string userPassword = list.FirstOrDefault(x => x.ConfigKey == Configuration.SMPTPASSWORD)?.ConfigValue;
            string host = list.FirstOrDefault(x => x.ConfigKey == Configuration.SMPTHOST)?.ConfigValue;
            string port = list.FirstOrDefault(x => x.ConfigKey == Configuration.SMPTPORT)?.ConfigValue;
            string isSSL = list.FirstOrDefault(x => x.ConfigKey == Configuration.SMPTISSSL)?.ConfigValue ?? "true";
            string displayName = list.FirstOrDefault(x => x.ConfigKey == Configuration.SMPTDISPLAYNAME)?.ConfigValue;

            // Log the actual configuration values
            _logger.LogInformation("Email configuration values: userName={userName}, userPassword={userPassword}, host={host}, port={port}, isSSL={isSSL}, displayName={displayName}", userName, userPassword, host, port, isSSL, displayName);

            // Check for missing configuration values
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword) || string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(isSSL))
            {
                _logger.LogError("Email configuration is missing required values.");
                throw new CustomException(ErrorResponseMessage.SendEmailFailed);
            }

            // Set up the MailMessage object
            msg.From = new MailAddress(userName, displayName);
            msg.To.Add(mailRequest.ToEmail); // Add recipient
            msg.Subject = mailRequest.Subject;
            msg.Body = mailRequest.Body;
            msg.IsBodyHtml = true;

            // Set up the SmtpClient object
            var client = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(userName, userPassword),
                Port = int.Parse(port),
                Host = host,
                EnableSsl = bool.Parse(isSSL)
            };

            try
            {
                await client.SendMailAsync(msg);
                _logger.LogInformation("Successfully sent email to {ToEmail}", mailRequest.ToEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email: {Exception}", ex);
                throw;
            }

            return Unit.Value;

        }

    }
}