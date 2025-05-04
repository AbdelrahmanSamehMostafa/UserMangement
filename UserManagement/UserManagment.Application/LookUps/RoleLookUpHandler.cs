using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.LookUps
{
    public record RoleLookUpDTO() : IRequest<IEnumerable<LookUpDTO>>;

    public class RoleLookUpHandler : IRequestHandler<RoleLookUpDTO, IEnumerable<LookUpDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleLookUpHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<LookUpDTO>> Handle(RoleLookUpDTO request, CancellationToken cancellationToken)
        {
            var res = await _unitOfWork.Role.RolesLookUp(cancellationToken);
            if (res == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return res;
        }

    }
}