using UserManagment.Common.DTO.GroupRole;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IGroupRoleRepository
    {
        Task<GroupRole?> Create(GroupRole request);
        Task<int> GetRoleCountByGroupId(Guid GroupId);
        Task AddRange(Guid GroupId, List<Guid> RolesId, CancellationToken cancellationToken);
        Task<IEnumerable<GroupRolesCountDTO>> GetRoleCountsByGroupIds(IEnumerable<Guid> groupIds);
        Task<IEnumerable<LookUpDTO>> GetGroupsRolesAsync(Guid groupId, CancellationToken cancellationToken);
        Task<List<Guid>> GetGroupRoleIdsAsync(Guid userId, CancellationToken cancellationToken);
        Task RemoveGroupRolesAsync(Guid groupId, List<Guid> rolesIds, CancellationToken cancellationToken);
        Task<List<Guid>> GetDistinctRoleIdsByUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}
