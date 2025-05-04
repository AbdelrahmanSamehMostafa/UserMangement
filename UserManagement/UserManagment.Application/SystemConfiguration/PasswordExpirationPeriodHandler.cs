using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;
using static UserManagment.Application.SystemConfiguration.PasswordExpirationPeriodHandler;

namespace UserManagment.Application.SystemConfiguration
{

    public class PasswordExpirationPeriodHandler : IRequestHandler<PasswordExpirationPeriodDto, bool>
    {
        public record PasswordExpirationPeriodDto(int MaxDurationInMonth) : IRequest<bool>;

        private readonly IUnitOfWork _unitOfWork;

        public PasswordExpirationPeriodHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(PasswordExpirationPeriodDto request, CancellationToken cancellationToken)
        {
            if (request.MaxDurationInMonth <= 0)
            {
                 throw new CustomException(ErrorResponseMessage.Negative_Values);
            }
            var perod_Type = await _unitOfWork.Configuration.GetByKeyAsync(Configuration.PasswordExpirationPeriod_Type);
            if (perod_Type == null)
            {
                perod_Type = new Configuration
                {
                    ConfigType = Configuration.PasswordExpirationPeriod_Type,
                    ConfigKey = Configuration.PasswordExpirationPeriod_Key,
                    ConfigValue = request.MaxDurationInMonth.ToString(),
                };
                _ = await _unitOfWork.Configuration.InsertKeyValueAsync(perod_Type);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            }
            else
            {
                perod_Type.ConfigValue = request.MaxDurationInMonth.ToString();
                perod_Type.ConfigKey = Configuration.PasswordExpirationPeriod_Key;
                _ = await _unitOfWork.Configuration.SetKeyValue(perod_Type, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }


}
