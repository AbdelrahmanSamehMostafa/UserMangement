using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Domain.Models;

namespace UserManagment.Application.DTOMapping
{
    public static class ScreenMapping
    {
        public static ScreenMenuResponseDto ToScreenMenuResponseDto(this Screen screen)
        {
            return new ScreenMenuResponseDto
            {
                ScreenId = screen.Id,
                ScreenName = screen.Name,
                AreaName = screen.AreaName,
                ParentId = screen.ParentId,
                IsMenuScreen = screen.IsMenuScreen
            };
        }
    }
}
