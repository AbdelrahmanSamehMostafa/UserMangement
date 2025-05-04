using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.GroupRecords;
using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.DTO.SearchInputs;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Group
{
    public record GroupGetAllDTO(GroupInputSearch GroupInputSearch) : IRequest<GroupGetAllResult>;
    public class GroupGetAllHandler : IRequestHandler<GroupGetAllDTO, GroupGetAllResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupGetAllHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GroupGetAllResult> Handle(GroupGetAllDTO request, CancellationToken cancellationToken)
        {
            var _data = await _unitOfWork.Group.GetGroupsAsync(request.GroupInputSearch, cancellationToken);
            if (!_data.Groups.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            // Get all role and user counts at once, outside of the loop
            var groupIds = _data.Groups.Select(g => g.Id).ToList();

            var roleCounts = await _unitOfWork.GroupRole.GetRoleCountsByGroupIds(groupIds);
            var userCounts = await _unitOfWork.GroupUser.GetUserCountsByGroupIds(groupIds);


            List<GroupListDTO> groupListDTO = new List<GroupListDTO>();

            foreach (var group in _data.Groups)
            {
                groupListDTO.Add(new GroupListDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    Code = group.Code,
                    Description = group.Description,
                    CountOfRoles = roleCounts.FirstOrDefault(rc => rc.GroupId == group.Id)?.RolesCount ?? 0,
                    CountOfUsers = userCounts.FirstOrDefault(uc => uc.GroupId == group.Id)?.UsersCount ?? 0
                });
            }
            var paginatedGroups = groupListDTO
                .Skip((request.GroupInputSearch.PageNumber - 1) * request.GroupInputSearch.PageSize)
                .Take(request.GroupInputSearch.PageSize)
                .ToList();

            var data = GroupMapping.ToGroupGetAllResult(paginatedGroups, _data.Count);

            return data;
        }
    }
}