using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Group
{
    public record GroupGetByIdDTO(Guid id) : IRequest<GroupDetailsDto>;
    public class GroupGetByIdHandler : IRequestHandler<GroupGetByIdDTO, GroupDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupGetByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GroupDetailsDto> Handle(GroupGetByIdDTO request, CancellationToken cancellationToken)
        {
            var group = await _unitOfWork.Group.GetById(request.id);
            if (group == null)
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            GroupDetailsDto groupDetailsDTO = new GroupDetailsDto
            {
                Id = group.Id,
                Name = group.Name,
                Code = group.Code,
                Description = group.Description,
                CountOfRoles = await _unitOfWork.GroupRole.GetRoleCountByGroupId(group.Id),
                CountOfUsers = await _unitOfWork.GroupUser.GetUsersCountByGroupId(group.Id)
            };

            groupDetailsDTO.Roles = await _unitOfWork.GroupRole.GetGroupsRolesAsync(group.Id, cancellationToken);
            groupDetailsDTO.Users = await _unitOfWork.GroupUser.GetUsersByGroupIdAsync(group.Id, cancellationToken);
            return groupDetailsDTO;
        }
    }
}