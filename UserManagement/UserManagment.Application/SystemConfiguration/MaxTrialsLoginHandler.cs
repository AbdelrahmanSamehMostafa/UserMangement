using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;
using static UserManagment.Application.SystemConfiguration.MaxTrialsLoginHandler;

namespace UserManagment.Application.SystemConfiguration
{

    public class MaxTrialsLoginHandler : IRequestHandler<MaxTrialsLoginDto, bool>
    {
        public record MaxTrialsLoginDto(int MaxTrial, int MaxDurationInMinutes) : IRequest<bool>;

        private readonly IUnitOfWork _unitOfWork;

        public MaxTrialsLoginHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(MaxTrialsLoginDto request, CancellationToken cancellationToken)
        {
            if (request.MaxTrial < 0 || request.MaxDurationInMinutes < 0)
                throw new CustomException(ErrorResponseMessage.Negative_Values);

            // Assuming you are retrieving or setting values for MaxTrial and MaxDurationInMinutes
            var maxTrial_TYPEModel = await _unitOfWork.Configuration.GetByKeyAsync(Configuration.MaxTrial_TYPE);
            if (maxTrial_TYPEModel == null)
            {
                await _unitOfWork.StartTransactionAsync(cancellationToken);
                maxTrial_TYPEModel = new Configuration
                {
                    ConfigType = Configuration.MaxTrial_TYPE,
                    ConfigKey = Configuration.MaxTrial_Key_MaxTrial,
                    ConfigValue = request.MaxTrial.ToString(),
                };

                _ = await _unitOfWork.Configuration.InsertKeyValueAsync(maxTrial_TYPEModel);


                maxTrial_TYPEModel.ConfigValue = request.MaxDurationInMinutes.ToString();
                maxTrial_TYPEModel.ConfigKey = Configuration.MaxTrial_Key_MaxDurationInMinutes;
                _ = await _unitOfWork.Configuration.InsertKeyValueAsync(maxTrial_TYPEModel);

                await _unitOfWork.SubmitTransactionAsync(cancellationToken);

            }
            else
            {
                await _unitOfWork.StartTransactionAsync(cancellationToken);

                maxTrial_TYPEModel.ConfigValue = request.MaxTrial.ToString();
                maxTrial_TYPEModel.ConfigKey = Configuration.MaxTrial_Key_MaxTrial;
                _ = await _unitOfWork.Configuration.SetKeyValue(maxTrial_TYPEModel, cancellationToken);

                maxTrial_TYPEModel.ConfigValue = request.MaxDurationInMinutes.ToString();
                maxTrial_TYPEModel.ConfigKey = Configuration.MaxTrial_Key_MaxDurationInMinutes;
                _ = await _unitOfWork.Configuration.SetKeyValue(maxTrial_TYPEModel, cancellationToken);
                await _unitOfWork.SubmitTransactionAsync(cancellationToken);
            }

            return true;
        }
    }


}
