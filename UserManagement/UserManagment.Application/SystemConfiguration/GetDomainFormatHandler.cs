using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Application.SystemConfiguration
{
    public record DomainFormat : IRequest<string>;
    public class GetDomainFormatHandler : IRequestHandler<DomainFormat, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDomainFormatHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DomainFormat request, CancellationToken cancellationToken)
        {
            var result= (await _unitOfWork.Configuration.GetByKeyAsync(Configuration.DomainFormat_Key)).ConfigValue;
            if (result == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return result;
        }
    }
}
