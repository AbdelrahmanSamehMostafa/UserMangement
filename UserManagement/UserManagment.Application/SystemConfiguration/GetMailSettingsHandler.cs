using System.Data;
using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.MailConfigurationDto;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.SystemConfiguration
{
    public record EmailConfigurationGetAll() : IRequest<List<ConfigurationDTo>>;

    public class GetMailSettingsHandler : IRequestHandler<EmailConfigurationGetAll, List<ConfigurationDTo>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMailSettingsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ConfigurationDTo>> Handle(EmailConfigurationGetAll request, CancellationToken cancellationToken)
        {
            var emailConfigurations = await _unitOfWork.Configuration.GetEmailConfigurationsAsync();
            if (!emailConfigurations.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            var emailConfigurationDtos = emailConfigurations.Select(config => new ConfigurationDTo
            {
                ConfigKey = config.ConfigKey,
                ConfigValue = config.ConfigValue,
            }).ToList();

            return emailConfigurationDtos;
        }

    }

}