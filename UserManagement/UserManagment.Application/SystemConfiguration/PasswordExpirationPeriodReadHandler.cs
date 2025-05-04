using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Application.SystemConfiguration
{
    public record PasswordExpirationPeriodRequest : IRequest<PasswordExpirationResponse>;
    public class PasswordExpirationPeriodReadHandler : IRequestHandler<PasswordExpirationPeriodRequest, PasswordExpirationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public PasswordExpirationPeriodReadHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PasswordExpirationResponse> Handle(PasswordExpirationPeriodRequest request, CancellationToken cancellationToken)
        {

            var result = new PasswordExpirationResponse(int.Parse((await _unitOfWork.Configuration.GetByKeyAsync(Configuration.PasswordExpirationPeriod_Key)).ConfigValue));
            if (result == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return result;
        }

    }
    public record PasswordExpirationResponse(int maxDurationInMonth);
}
