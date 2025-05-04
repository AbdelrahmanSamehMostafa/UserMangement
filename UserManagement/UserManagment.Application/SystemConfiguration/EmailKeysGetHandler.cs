using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.SystemConfiguration
{
    public record GetEmailKeysRequest:IRequest<List<string>>;
    public class EmailKeysGetHandler:IRequestHandler<GetEmailKeysRequest, List<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmailKeysGetHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> Handle(GetEmailKeysRequest request, CancellationToken cancellationToken)
        {
            var config = await _unitOfWork.Configuration.GetByTypeAsync("emailtemplate");
            var emailKeys = config.Select(c => c.ConfigKey).ToList();
            if (!emailKeys.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return emailKeys;
        }
    }
}
