using UserManagment.Common.DTO.Common;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IScreenActionRepository
    {
        Task<ScreenAction?> GetById(Guid id);
        Task<IEnumerable<ScreenActionDTO>> GetScreensActions();
        Task<bool> CheckforScreenAction(List<Guid> screenActionIds);
        Task<List<string?>> GetPoliciesNamebyScreenIds(List<Guid> screenIds, CancellationToken cancellationToken);
    }
}
