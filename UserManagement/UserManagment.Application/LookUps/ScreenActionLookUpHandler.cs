using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.Helpers;


namespace UserManagment.Application.LookUps
{
    public record ScreenActionsGetAllDTO() : IRequest<IEnumerable<ScreenActionDTO>>;

    public class ScreenActionLookUpHandler : IRequestHandler<ScreenActionsGetAllDTO, IEnumerable<ScreenActionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScreenActionLookUpHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ScreenActionDTO>> Handle(ScreenActionsGetAllDTO request, CancellationToken cancellationToken)
        {
            var res = await _unitOfWork.ScreenAction.GetScreensActions();
            if (res == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return res;

        }
    }
}