using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Domain.Models;

namespace UserManagment.Application.DTOMapping
{
    public static class RoleScreenActionMapping
    {
        public static AllowedActionDto ToAllowedActionDto(this RoleScreenAction roleScreenAction)
        {
             var res = new AllowedActionDto
             {
                 ActionId = roleScreenAction.ScreenActionId,
                 ActionName = roleScreenAction.ScreenAction?.ActionDisplayName ?? string.Empty,
                 ActionType = roleScreenAction.ScreenAction?.ActionType ?? default
             };
            return res;
        }
    }
}
