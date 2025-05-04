using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.LookUps
{
    public record ScreenGetAllDTO() : IRequest<IEnumerable<LookUpDTO>>;

    public class ScreenLookUpHandler : IRequestHandler<ScreenGetAllDTO, IEnumerable<LookUpDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScreenLookUpHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<LookUpDTO>> Handle(ScreenGetAllDTO request, CancellationToken cancellationToken)
        {
            var res = _unitOfWork.Screen.GetSCreenLookUp();
            if (res == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return res;
        }
    }
}