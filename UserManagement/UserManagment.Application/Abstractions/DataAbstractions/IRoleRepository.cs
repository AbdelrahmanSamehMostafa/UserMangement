using UserManagment.Application.RoleRecords;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.DTO.RoleDTo;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IRoleRepository
    {
        Task<Role?> Create(Role request);
        Task<bool> IsFound(RoleDto request);
        Task<bool> DefaultRoleExist();
        Task<Role?> GetDefaultRole();
        Task<Role?> Update(RoleDto request);
        Task<(IEnumerable<RoleResultDto> Rolses, int Count)> GetRolesAsync(BaseListingInput baseListingInput, CancellationToken cancellationToken);
        Task<Role?> GetById(Guid id);
        Task<IEnumerable<LookUpDTO>> RolesLookUp(CancellationToken cancellationToken);
    }
}
