using UserManagment.Application.Users;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.DTO.GroupUserDTO;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IGroupUserRepository
    {
        Task RemoveUserGroupsAsync(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken);
        Task<GroupUser?> Create(GroupUser request);
        Task<IEnumerable<LookUpDTO>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> IsUserInGroups(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken);
        Task addGroupsUserAsync(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken);
        Task addGroupUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
        Task<IEnumerable<Common.DTO.GroupUserDTO.UserGroupsDTO>> GetGroupsForExportAsync(List<Guid> userId, CancellationToken cancellationToken);
        Task<int> GetUsersCountByGroupId(Guid groupId);
        Task<IEnumerable<GroupUsersCountDTO>> GetUserCountsByGroupIds(IEnumerable<Guid> groupIds);
        Task<List<Guid>> GetUserGroupIdsAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<UserLookupDto>> GetUsersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    }
}
