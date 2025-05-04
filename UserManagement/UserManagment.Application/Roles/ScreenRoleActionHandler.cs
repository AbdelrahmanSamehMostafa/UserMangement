using MediatR;
using UserManagement.Common.Helpers;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.RoleDTo;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Roles
{
    public record ScreenRoleDTO(ScreenRolesDTO ScreenRolesDTO) : IRequest<bool>;
    public class ScreenRoleActionHandler : IRequestHandler<ScreenRoleDTO, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ScreenRoleActionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(ScreenRoleDTO request, CancellationToken cancellationToken)
        {
            var screenExists = await _unitOfWork.ScreenAction.CheckforScreenAction(request.ScreenRolesDTO.ScreenActionIds);
            if (!screenExists)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            var role = await _unitOfWork.Role.GetById(request.ScreenRolesDTO.RoleId);
            
            if (role == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            var existingScreenActionsIds = await _unitOfWork.RoleScreenAction.GetRoleScreenActions(request.ScreenRolesDTO.RoleId, cancellationToken);
            // Call the generic function to update the groups
            await GenericFunctions.UpdateCollectionAsync(
                existingScreenActionsIds,
                request.ScreenRolesDTO.ScreenActionIds,
                _unitOfWork.RoleScreenAction.AddScreenRoleAction,
                _unitOfWork.RoleScreenAction.DeleteScreenRoleAction,
                request.ScreenRolesDTO.RoleId,
                cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }


    }
}