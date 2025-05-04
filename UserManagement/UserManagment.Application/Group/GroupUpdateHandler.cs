using MediatR;
using UserManagement.Common.Helpers;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Group
{
    public record GroupUpdateDTO(GroupForUpdateDto RequestDto) : IRequest<bool>;
    public class GroupUpdateHandler : IRequestHandler<GroupUpdateDTO, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupUpdateHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(GroupUpdateDTO request, CancellationToken cancellationToken)
        {
            var group = await _unitOfWork.Group.GetById(request.RequestDto.Id);
            if (group is null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            group.Name = request.RequestDto.Name;
            group.Code = request.RequestDto.Code;
            group.Description = request.RequestDto.Description;

            if (request.RequestDto.RolesIds is null || !request.RequestDto.RolesIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.NoRoles);
            }
            // Fetch existing roles IDs
            var existingRoleIds = await _unitOfWork.GroupRole.GetGroupRoleIdsAsync(request.RequestDto.Id, cancellationToken);

            // Call the generic function to update the roles
            await GenericFunctions.UpdateCollectionAsync(
                existingRoleIds,
                request.RequestDto.RolesIds,
                _unitOfWork.GroupRole.AddRange,
                _unitOfWork.GroupRole.RemoveGroupRolesAsync,
                request.RequestDto.Id,
                cancellationToken);

            try
            {
                await _unitOfWork.Group.Update(group);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch
            {
                throw new CustomException(ErrorResponseMessage.InternalServerError);
            }

        }
    }
}