using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.ScreenMenu
{
    public record ScreenPermissionRequest(Guid ScreedId, Guid UserId) : IRequest<List<AllowedActionDto>?>;
    public class ScreenPermissionGetHandler(IUnitOfWork unitOfWork) : IRequestHandler<ScreenPermissionRequest, List<AllowedActionDto>?>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<AllowedActionDto>?> Handle(ScreenPermissionRequest request, CancellationToken cancellationToken)
        {
            var res = await _unitOfWork.RoleScreenAction.GetAllowedActions(request.ScreedId, request.UserId, cancellationToken);
            if (res == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            return res;
        }
    }
}
