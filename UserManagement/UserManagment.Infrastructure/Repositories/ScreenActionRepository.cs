using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.Common;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure.Repositories
{
    public class ScreenActionRepository(UserManagmentDbContext ctx) : IScreenActionRepository
    {
        public async Task<ScreenAction?> GetById(Guid id)
        {
            return await ctx.ScreenAction.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ScreenActionDTO>> GetScreensActions()
        {
            var screenActions = await ctx.ScreenAction.AsNoTracking()
                .GroupBy(e => new { e.Screen.Id, e.Screen.Name })
                .Select(g => new ScreenActionDTO
                {
                    ScreenId = g.Key.Id,
                    ScreenName = g.Key.Name,
                    actions = g.Select(a => new Actions
                    {
                        Id = a.Id,
                        ActionType = a.ActionType,
                        ActionName = a.ActionDisplayName
                    }).ToList()
                }).ToListAsync();

            return screenActions;
        }
        public async Task<bool> CheckforScreenAction(List<Guid> screenActionIds)
        {
            return await ctx.ScreenAction.AnyAsync(e => screenActionIds.Contains(e.Id));
        }

        public async Task<List<string?>> GetPoliciesNamebyScreenIds(List<Guid> screenIds, CancellationToken cancellationToken)
        {
            return await ctx.ScreenAction.AsNoTracking()
                .Where(e => screenIds.Contains(e.ScreenId))
                .Select(e => e.PolicyName)
                .ToListAsync();
        }
    }
}
