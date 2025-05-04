using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.GroupRecords;
using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Group
{
    public record GroupCreateDTO(GroupForCreateDTO Group) : IRequest<GroupResultDto>;
    public class GroupCreaterHandler : IRequestHandler<GroupCreateDTO, GroupResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupCreaterHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GroupResultDto> Handle(GroupCreateDTO request, CancellationToken cancellationToken)
        {
            if (request.Group is null)
            {
                throw new CustomException(ErrorResponseMessage.NullRequest);
            }
            if(request.Group.RolesIds is null || !request.Group.RolesIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.NoRoles);
            }
            GroupDTO groupDto = new GroupDTO(Guid.NewGuid(), request.Group.Name, request.Group.Code, request.Group.Description);

            bool isFound = await _unitOfWork.Group.IsFound(groupDto);
            if (isFound)
            {
                throw new CustomException(ErrorResponseMessage.isRepeated);
            }
            else
            {
                var newGroup = new Domain.Models.Group
                {
                    Name = request.Group.Name,
                    Code = request.Group.Code,
                    Description = request.Group.Description,
                    Id = Guid.NewGuid()
                };
                var res = await _unitOfWork.Group.Create(newGroup);
                await _unitOfWork.GroupRole.AddRange(newGroup.Id, request.Group.RolesIds, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var groupResultDto = GroupMapping.ToGroupResult(res);
                return groupResultDto;
            }
        }
    }
}