using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.GroupUserDTO;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure.Repositories
{
    public class GroupUserRepository(UserManagmentDbContext ctx) : IGroupUserRepository
    {
        public async Task RemoveUserGroupsAsync(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken)
        {
            // Fetch the GroupUser entries that match the provided userId and groupIds
            var groupUsersToRemove = await ctx.GroupUsers
                .Where(gu => gu.UserId == userId && groupIds.Contains(gu.GroupId))
                .ToListAsync(cancellationToken);

            // Remove the entries from the context
            ctx.GroupUsers.RemoveRange(groupUsersToRemove);
        }

        public async Task addGroupsUserAsync(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken)
        {
            await ctx.GroupUsers.AddRangeAsync(groupIds.Select(g => new GroupUser { UserId = userId, GroupId = g }), cancellationToken);
        }

        public async Task addGroupUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            await ctx.GroupUsers.AddAsync(new GroupUser { UserId = userId, GroupId = groupId }, cancellationToken);
        }

        public async Task<GroupUser?> Create(GroupUser request)
        {
            var res = await ctx.GroupUsers.AddAsync(request);
            return res.Entity;
        }

        public async Task<List<Guid>> GetUserGroupIdsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await ctx.GroupUsers.AsNoTracking()
                .Where(gu => gu.UserId == userId)
                .Select(gu => gu.GroupId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LookUpDTO>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await ctx.GroupUsers.AsNoTracking()
                .Where(gu => gu.UserId == userId)
                .Include(gu => gu.Group)
                .Select(gu => new LookUpDTO { Id = gu.Group.Id, Name = gu.Group.Name })
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<UserLookupDto>> GetUsersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        {
            return await ctx.GroupUsers.AsNoTracking()
                .Where(gu => gu.GroupId == groupId)
                .Include(gu => gu.User)
                .Select(gu => new UserLookupDto
                {
                    Id = gu.User.Id,
                    Name = gu.User.FirstName + " " + gu.User.LastName,
                    Email = gu.User.Email,
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUserInGroups(Guid userId, List<Guid> groupIds, CancellationToken cancellationToken)
        {
            return await ctx.GroupUsers.AnyAsync(gu => gu.UserId == userId && groupIds.Contains(gu.GroupId), cancellationToken);
        }

        public async Task<int> GetUsersCountByGroupId(Guid groupId)
        {
            return await ctx.GroupUsers.AsNoTracking().CountAsync(e => e.GroupId == groupId && e.User.IsDeleted == false);
        }

        public async Task<IEnumerable<GroupUsersCountDTO>> GetUserCountsByGroupIds(IEnumerable<Guid> groupIds)
        {
            return await ctx.GroupUsers.AsNoTracking()
                .Where(gu => groupIds.Contains(gu.GroupId) && gu.User.IsDeleted == false)
                .GroupBy(gu => gu.GroupId)
                .Select(g => new GroupUsersCountDTO { GroupId = g.Key, UsersCount = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<Common.DTO.GroupUserDTO.UserGroupsDTO>> GetGroupsForExportAsync(List<Guid> userIds, CancellationToken cancellationToken)
        {
            // Retrieve all groups for the users in the provided list of userIds
            var groupUsers = await ctx.GroupUsers.AsNoTracking()
                .Where(gu => userIds.Contains(gu.UserId))
                .Include(gu => gu.Group)
                .ToListAsync(cancellationToken);

            // Group by UserId and create UserGroupsDTO for each user
            var userGroupsDto = groupUsers
                .GroupBy(gu => gu.UserId)
                .Select(group => new Common.DTO.GroupUserDTO.UserGroupsDTO
                {
                    UserId = group.Key, // UserId from the group
                    Groups = group.Any()
                        ? $"{{ {string.Join(", ", group.Select(gu => gu.Group.Name))} }}"
                        : string.Empty // Return an empty string if there are no groups
                })
                .ToList();

            return userGroupsDto;
        }
    }
}
