using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.GroupRole;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure.Repositories
{
    public class GroupRoleRepository(UserManagmentDbContext ctx) : IGroupRoleRepository
    {
        public async Task<GroupRole?> Create(GroupRole request)
        {
            var res = await ctx.GroupRoles.AddAsync(request);
            return res.Entity;
        }

        public async Task<List<Guid>> GetGroupRoleIdsAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return await ctx.GroupRoles.AsNoTracking()
                .Where(gu => gu.GroupId == groupId)
                .Select(gu => gu.RoleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Guid>> GetDistinctRoleIdsByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Fetch all distinct group IDs the user belongs to
            var userGroupIds = await ctx.GroupUsers
            .AsNoTracking()
                .Where(gu => gu.UserId == userId)
                .Select(gu => gu.GroupId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!userGroupIds.Any())
            {
                // If the user is not part of any groups, return an empty list
                return new List<Guid>();
            }

            // Fetch distinct role IDs associated with the user's groups
            var roleIds = await ctx.GroupRoles
                .AsNoTracking()
                .Where(gr => userGroupIds.Contains(gr.GroupId))
                .Select(gr => gr.RoleId)
                .Distinct()
                .ToListAsync(cancellationToken);

            return roleIds;
        }

        public async Task<int> GetRoleCountByGroupId(Guid GroupId)
        {
            return await ctx.GroupRoles.AsNoTracking().CountAsync(e => e.GroupId == GroupId && e.Role.IsDeleted == false);
        }

        public async Task<IEnumerable<GroupRolesCountDTO>> GetRoleCountsByGroupIds(IEnumerable<Guid> groupIds)
        {
            return await ctx.GroupRoles.AsNoTracking()
                .Where(gr => groupIds.Contains(gr.GroupId) && gr.Role.IsDeleted == false)
                .GroupBy(gr => gr.GroupId)
                .Select(g => new GroupRolesCountDTO { GroupId = g.Key, RolesCount = g.Count() })
                .ToListAsync();
        }

        public async Task AddRange(Guid GroupId, List<Guid> RolesId, CancellationToken cancellationToken)
        {
            await ctx.GroupRoles.AddRangeAsync(RolesId.Select(g => new GroupRole { GroupId = GroupId, RoleId = g }), cancellationToken);
        }

        public async Task<IEnumerable<LookUpDTO>> GetGroupsRolesAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return await ctx.GroupRoles.AsNoTracking()
                .Where(gr => gr.GroupId == groupId)
                .Include(gr => gr.Role)
                .Select(gr => new LookUpDTO { Id = gr.Role.Id, Name = gr.Role.Name })
                .ToListAsync(cancellationToken);
        }

        public async Task RemoveGroupRolesAsync(Guid groupId, List<Guid> rolesIds, CancellationToken cancellationToken)
        {
            var groupRolesToRemove = await ctx.GroupRoles
                .Where(gu => gu.GroupId == groupId && rolesIds.Contains(gu.RoleId))
                .ToListAsync(cancellationToken);

            // Remove the entries from the context
            ctx.GroupRoles.RemoveRange(groupRolesToRemove);
        }
    }
}
