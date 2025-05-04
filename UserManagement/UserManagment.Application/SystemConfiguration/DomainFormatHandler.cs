using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;
using static UserManagment.Application.SystemConfiguration.DomainFormatHandler;

namespace UserManagment.Application.SystemConfiguration
{

    public class DomainFormatHandler : IRequestHandler<DomainFormatDto, bool>
    {
        public record DomainFormatDto(string allowedDomains) : IRequest<bool>;

        private readonly IUnitOfWork _unitOfWork;

        public DomainFormatHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DomainFormatDto request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.allowedDomains))
            {
                throw new CustomException(ErrorResponseMessage.DomainFormatRequired);
            }
            var domains = request.allowedDomains.Split(';');
            foreach (var domain in domains)
            {
                if (!domain.Contains("."))
                {
                    throw new CustomException(ErrorResponseMessage.User_MailDomain);
                }
            }
            
            // Assuming you are retrieving or setting values for MaxTrial and MaxDurationInMinutes
            var maxTrial_TYPEModel = await _unitOfWork.Configuration.GetByKeyAsync(Configuration.DomainFormat_Key);
            if (maxTrial_TYPEModel == null)
            {
                maxTrial_TYPEModel = new Configuration
                {
                    ConfigType = Configuration.DomainFormat_Type,
                    ConfigKey = Configuration.DomainFormat_Key,
                    ConfigValue = request.allowedDomains,
                };
                _ = await _unitOfWork.Configuration.InsertKeyValueAsync(maxTrial_TYPEModel);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                try
                {
                    if (maxTrial_TYPEModel.ConfigValue != request.allowedDomains)
                    {
                        maxTrial_TYPEModel.ConfigValue = request.allowedDomains;
                    };
                    return await _unitOfWork.Configuration.SetKeyValue(maxTrial_TYPEModel, cancellationToken) > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return true;
        }
    }


}
