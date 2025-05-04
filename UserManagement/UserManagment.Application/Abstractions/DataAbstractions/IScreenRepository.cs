using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IScreenRepository
    {
        Task <IEnumerable<LookUpDTO>>GetSCreenLookUp();
        Task<Screen?>GetScreenById(Guid id);
        Task<List<ScreenMenuResponseDto>> GetAllScreensWithIds(List<Guid> screenIds);
    }
}
