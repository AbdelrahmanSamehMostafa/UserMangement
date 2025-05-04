using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IRoleScreenActionRepository
    {
        Task<List<Guid>> GetDistinctScreenIdsByRoleIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken);
        Task<RoleScreenAction?> Create(RoleScreenAction request);
        Task<List<Guid>?> GetRoleScreenActions(Guid RoleId, CancellationToken cancellationToken);
        Task AddScreenRoleAction(Guid RoleId, List<Guid> ScreenActionIds, CancellationToken cancellationToken);
        Task DeleteScreenRoleAction(Guid roleId, List<Guid> screenActionIds, CancellationToken cancellationToken);
        Task<List<AllowedActionDto>?> GetAllowedActions(Guid ScreenId, Guid UserId, CancellationToken cancellationToken);
    }
}
