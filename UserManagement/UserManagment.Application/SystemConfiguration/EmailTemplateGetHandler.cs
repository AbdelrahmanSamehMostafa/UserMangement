using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.SystemConfiguration
{
    public record GetEmailTemplateRequest(string ConfigKey):IRequest<EmailTemplateDto>;
    public class EmailTemplateGetHandler : IRequestHandler<GetEmailTemplateRequest, EmailTemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmailTemplateGetHandler(IUnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }
        public async Task<EmailTemplateDto> Handle(GetEmailTemplateRequest request, CancellationToken cancellationToken)
        {
            var config = (await _unitOfWork.Configuration.GetByTypeAsync("emailtemplate")).FirstOrDefault(c => c.ConfigKey.ToLower() == request.ConfigKey.Trim().ToLower());
            if (config == null) throw new CustomException(ErrorResponseMessage.InvalidConfigKey);
            var emailTemplate = new EmailTemplateDto(config.ConfigKey, config.ConfigValue, config.ConfigType);
            return emailTemplate;
        }
    }

    public record EmailTemplateDto(string ConfigKey, string ConfigValue, string ConfigType);
}
