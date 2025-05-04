using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.MailConfigurationDto;
using UserManagment.Domain.Models;

namespace UserManagment.Application.SystemConfiguration
{
    public record MailConfigurationRequest(List<ConfigurationDTo> ConfigDtos) : IRequest<bool>;

    // Request to update or insert configuration setting
    public class MailConfigurationHandler : IRequestHandler<MailConfigurationRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MailConfigurationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(MailConfigurationRequest request, CancellationToken cancellationToken)
        {
            foreach (var configDto in request.ConfigDtos)
            {
                // Fetch the existing configuration
                var config = await _unitOfWork.Configuration.GetByKeyAndTypeAsync(configDto.ConfigKey, configDto.ConfigType);

                if (config == null)
                {
                    // Insert new configuration if it doesn't exist
                    config = new Configuration
                    {
                        ConfigKey = configDto.ConfigKey,
                        ConfigType = configDto.ConfigType,
                        ConfigValue = configDto.ConfigValue,
                        InsertedDate = DateTime.Now,
                    };
                    await _unitOfWork.Configuration.InsertKeyValueAsync(config);
                }
                else
                {
                    // Update the existing configuration
                    config.ConfigValue = configDto.ConfigValue;
                    config.UpdatedDate = DateTime.Now;
                }
            }

            // Save changes to the database after processing all configurations
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}