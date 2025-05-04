using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Application.SystemConfiguration
{
    public record MaxTrialsLoginRequest : IRequest<MaxTrialsLoginResponse>;
    public class MaxTrialsLoginGetHandler : IRequestHandler<MaxTrialsLoginRequest, MaxTrialsLoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public MaxTrialsLoginGetHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MaxTrialsLoginResponse> Handle(MaxTrialsLoginRequest request, CancellationToken cancellationToken)
        {
            var maxDuration = int.Parse((await _unitOfWork.Configuration.GetByKeyAsync(Configuration.MaxTrial_Key_MaxDurationInMinutes)).ConfigValue);
            var maxTrial = int.Parse((await _unitOfWork.Configuration.GetByKeyAsync(Configuration.MaxTrial_Key_MaxTrial)).ConfigValue);
            
            if (maxDuration == 0 || maxTrial == 0)
                throw new CustomException(ErrorResponseMessage.NotFound);

            return new MaxTrialsLoginResponse(maxDuration, maxTrial);
        }
    }
    public record MaxTrialsLoginResponse(int MaxDurationInMinutes, int MaxTrial);
}
