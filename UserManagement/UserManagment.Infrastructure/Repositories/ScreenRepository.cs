using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Domain.Models;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Application.DTOMapping;

namespace UserManagment.Infrastructure.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly UserManagmentDbContext _ctx;

        public ScreenRepository(UserManagmentDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<LookUpDTO>> GetSCreenLookUp()
        {
            return await _ctx.Screens.AsNoTracking().Select(s => new LookUpDTO { Id = s.Id, Name = s.Name }).ToListAsync();
        }
        public async Task<Screen?> GetScreenById(Guid id)
        {
            return await _ctx.Screens.FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<List<ScreenMenuResponseDto>> GetAllScreensWithIds(List<Guid> screenIds)
        {
            return await _ctx.Screens
                .AsNoTracking()
                .Where(s => screenIds.Contains(s.Id))
                .Select(s => ScreenMapping.ToScreenMenuResponseDto(s))
                .ToListAsync();
        }

    }
}
