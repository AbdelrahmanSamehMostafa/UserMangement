using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure.Repositories
{
    public class RoleScreenActionRepository(UserManagmentDbContext ctx) : IRoleScreenActionRepository
    {
        public async Task AddScreenRoleAction(Guid RoleId, List<Guid> ScreenActionIds, CancellationToken cancellationToken)
        {
            {
                await ctx.RoleScreenActions.AddRangeAsync(ScreenActionIds.Select(g => new RoleScreenAction { RoleId = RoleId, ScreenActionId = g }), cancellationToken);

            }
        }
        public async Task DeleteScreenRoleAction(Guid RoleId, List<Guid> ScreenActionIds, CancellationToken CancellationToken)
        {
            // Validate inputs
            if (RoleId == Guid.Empty || ScreenActionIds == null || !ScreenActionIds.Any())
            {
                throw new ArgumentException("RoleId and ScreenActionIds must be provided.");
            }
            var actionsToUpdate = await ctx.RoleScreenActions
                .Where(rsa => rsa.RoleId == RoleId && ScreenActionIds.Contains(rsa.ScreenActionId))
                .ToListAsync(CancellationToken);

            foreach (var action in actionsToUpdate)
            {
                action.IsDeleted = true;
            }
            await ctx.SaveChangesAsync(CancellationToken);
        }

        public async Task<List<Guid>?> GetRoleScreenActions(Guid RoleId, CancellationToken cancellationToken)
        {
            return await ctx.RoleScreenActions.AsNoTracking()
            .Where(sa => sa.RoleId == RoleId && sa.IsDeleted == false)
            .Select(sa => sa.ScreenActionId).ToListAsync(cancellationToken);
        }

        public async Task<RoleScreenAction?> Create(RoleScreenAction request)
        {
            var res = await ctx.RoleScreenActions.AddAsync(request);
            return res.Entity;
        }

        public async Task<List<Guid>> GetDistinctScreenIdsByRoleIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken)
        {
            if (roleIds == null || !roleIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            // Use Exists sub-query instead of join for better performance with large datasets
            return await ctx.ScreenAction
                .AsNoTracking()
                .Where(sa => !sa.IsDeleted &&
                             ctx.RoleScreenActions
                                .AsNoTracking()
                                .Where(rsa => roleIds.Contains(rsa.RoleId) && !rsa.IsDeleted)
                                .Select(rsa => rsa.ScreenActionId)
                                .Contains(sa.Id))
                .Select(sa => sa.ScreenId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AllowedActionDto>?> GetAllowedActions(Guid screenId, Guid userId, CancellationToken cancellationToken)
        {

            // Step 1: Get the roles for the user's groups
            var userGroups = await ctx.GroupUsers
            .AsNoTracking()
                .Where(ug => ug.UserId == userId)
                .Select(ug => ug.GroupId)
                .ToListAsync(cancellationToken);

            var userRoles = await ctx.GroupRoles.AsNoTracking()
                .Where(gr => userGroups.Contains(gr.GroupId))
                .Select(gr => gr.RoleId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Step 2: Get actions for the user's roles and filter by ScreenId
            var allowedActions = await ctx.RoleScreenActions.AsNoTracking()
            .Where(rsa => userRoles.Contains(rsa.RoleId) && rsa.ScreenAction.ScreenId == screenId)
            .Include(rsa => rsa.ScreenAction) // Ensure ScreenAction is loaded
            .Select(rsa => RoleScreenActionMapping.ToAllowedActionDto(rsa))
            .ToListAsync(cancellationToken);
            return allowedActions;
        }
        


    }
}
