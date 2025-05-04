using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Domain.Models;

namespace UserManagment.Application.SystemConfiguration
{
    // Request to update email template
    public record EmailTemplateRequest(string ConfigKey, string ConfigType, string NewTemplateBody) : IRequest<bool>;

    public class EmailTemplateHandler : IRequestHandler<EmailTemplateRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailTemplateHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(EmailTemplateRequest request, CancellationToken cancellationToken)
        {
            // Fetch the existing configuration
            var config = await _unitOfWork.Configuration.GetByKeyAndTypeAsync(request.ConfigKey, request.ConfigType);

            if (config == null)
            {
                // Insert new configuration if it doesn't exist
                config = new Configuration
                {
                    ConfigKey = request.ConfigKey,
                    ConfigType = request.ConfigType,
                    ConfigValue = request.NewTemplateBody,
                    InsertedDate = DateTime.Now,
                };
                await _unitOfWork.Configuration.InsertKeyValueAsync(config);
            }
            else
            {
                // Update the existing configuration
                config.ConfigValue = request.NewTemplateBody;
                config.UpdatedDate = DateTime.Now;
            }

            // Save changes to the database
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}