using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.DTO.RoleDTo;
using UserManagment.Domain.Models;
using Abp.Collections.Extensions;
using UserManagment.Application.RoleRecords;
using UserManagment.Common.DTO.LookUps;

namespace UserManagment.Infrastructure.Repositories
{
    public class RoleRepository(UserManagmentDbContext ctx) : IRoleRepository
    {
        public async Task<Role?> Create(Role request)
        {
            var res = await ctx.Roles.AddAsync(request);
            return res.Entity;
        }
        public async Task<bool> IsFound(RoleDto request)
        {
            var res = ctx.Roles.Any(e => e.Name == request.Name && e.Id != request.Id);
            return res;
        }
        public async Task<bool> DefaultRoleExist()
        {
            var res = await ctx.Roles.AnyAsync(e => e.IsDefault);
            return res;
        }
        public async Task<Role?> GetDefaultRole()
        {
            return await ctx.Roles.FirstOrDefaultAsync(R => R.IsDefault);
        }

        public Task<int> Delete(Configuration configuration, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<LookUpDTO>> RolesLookUp(CancellationToken cancellationToken)
        {
            return await ctx.Roles.AsNoTracking()
                .Select(role => new LookUpDTO
                {
                    Id = role.Id,
                    Name = role.Name
                }).ToListAsync();
        }
        public async Task<(IEnumerable<RoleResultDto> Rolses, int Count)> GetRolesAsync(BaseListingInput baseListingInput, CancellationToken cancellationToken)
        {
            var query = ctx.Roles.AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(baseListingInput.SearchString),
                    e =>
                    e.Name.Contains(baseListingInput.SearchString, StringComparison.OrdinalIgnoreCase));


            var roles = query.Select(role => role.ToRoleResult());
            // Get the total count before pagination
            int count = query.Count();

            return (roles, count);
        }
        public async Task<Role?> GetById(Guid id)
        {
            return await ctx.Roles.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Role?> Update(RoleDto request)
        {
            var row = await ctx.Roles.FirstOrDefaultAsync(e => e.Id == request.Id);
            row.Name = request.Name;
            row.IsDefault = request.IsDefault;
            return row;
        }


    }
}
