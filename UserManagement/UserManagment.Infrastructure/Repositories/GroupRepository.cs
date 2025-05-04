using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Domain.Models;
using Abp.Collections.Extensions;
using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.DTO.SearchInputs;

namespace UserManagment.Infrastructure.Repositories
{
    public class GroupRepository(UserManagmentDbContext ctx) : IGroupRepository
    {
        public async Task<Group?> Create(Group request)
        {
            var res = await ctx.Groups.AddAsync(request);
            return res.Entity;
        }
        public async Task<bool> IsFound(GroupDTO groupDTO)
        {
            var res = await ctx.Groups.AnyAsync(e => (e.Name == groupDTO.Name || e.Code == groupDTO.Code) && (e.Id != groupDTO.Id || e.Id == Guid.Empty));
            return res;
        }
        public async Task<(IEnumerable<GroupDTO> Groups, int Count)> GetGroupsAsync(GroupInputSearch baseListingInput,
         CancellationToken cancellationToken)
        {
            var sortExpression = Group.SortBy(baseListingInput.Sorting);

            var query = ctx.Groups.AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(baseListingInput.SearchString),
                    e =>
                    e.Name.Contains(baseListingInput.SearchString, StringComparison.OrdinalIgnoreCase));

            // Apply sorting based on the direction
            if (sortExpression.IsDescending)
            {
                query = query.AsQueryable().OrderByDescending(sortExpression.Expression);
            }
            else
            {
                query = query.AsQueryable().OrderBy(sortExpression.Expression);
            }
            int count = query.Count();
            var Groups = query.Select(a => new GroupDTO(a.Id, a.Name, a.Code, a.Description));
            return (Groups, count);
        }
        public async Task<Group?> GetById(Guid id)
        {
            return await ctx.Groups.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<(string Code, Guid Id)>> GetGroupIdsByCodesAsync(List<string> groupCodes)
        {
            return await ctx.Groups.AsNoTracking()
                            .Where(g => groupCodes.Contains(g.Code))
                            .Select(g => new { g.Code, g.Id })
                            .ToListAsync()
                            .ContinueWith(task => task.Result
                                .Select(g => (g.Code, g.Id))
                                .ToList());
        }


        public async Task<Group?> Update(Domain.Models.Group request)
        {
            var row = await ctx.Groups.FirstOrDefaultAsync(e => e.Id == request.Id);
            if (row != null)
            {
                row.Name = request.Name;
                row.Code = request.Code;
                row.Description = request.Description;

                ctx.Groups.Update(row);
                return row;
            }
            return null;
        }

        public async Task<IEnumerable<GroupsForExportDTO>> GetGroupsForExport(GroupInputSearch baseListingInput, CancellationToken cancellationToken)
        {
            var sortExpression = Group.SortBy(baseListingInput.Sorting);

            var query = ctx.Groups.AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(baseListingInput.SearchString),
                    e =>
                    e.Name.Contains(baseListingInput.SearchString, StringComparison.OrdinalIgnoreCase));

            // Apply sorting based on the direction
            if (sortExpression.IsDescending)
            {
                query = query.AsQueryable().OrderByDescending(sortExpression.Expression);
            }
            else
            {
                query = query.AsQueryable().OrderBy(sortExpression.Expression);
            }

            var Groups = query
                  .Select(a => new GroupsForExportDTO
                  {
                      Id = a.Id,
                      Name = a.Name,

                  }).Skip((baseListingInput.PageNumber - 1) * baseListingInput.PageSize)
                  .Take(baseListingInput.PageSize)
                  .ToList();

            return Groups;
        }
    }
}
