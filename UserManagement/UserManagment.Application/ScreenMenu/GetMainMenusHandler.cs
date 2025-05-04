using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Common.DTO.ScreenMenu;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.ScreenMenu
{

    // Request for get main menus
    public record GetMainMenusById(Guid userId) : IRequest<List<ScreenMenuResponseDto>?>;
    public class GetMainMenusHandler : IRequestHandler<GetMainMenusById, List<ScreenMenuResponseDto>?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMainMenusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ScreenMenuResponseDto>?> Handle(GetMainMenusById request, CancellationToken cancellationToken)
        {
            // Fetch distinct role IDs for the user
            var userRoleIds = await _unitOfWork.GroupRole.GetDistinctRoleIdsByUserAsync(request.userId, cancellationToken);
            if (userRoleIds == null || !userRoleIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.RolesUser_NotFound);

            }

            //fetch screen ids with these roles
            var ScreenIds = await _unitOfWork.RoleScreenAction.GetDistinctScreenIdsByRoleIdsAsync(userRoleIds, cancellationToken);
            if (ScreenIds == null || !ScreenIds.Any())
            {
                throw new CustomException(ErrorResponseMessage.RoleScreen_NotFound);
            }

            // For now, return some kind of mock response
            var response = await _unitOfWork.Screen.GetAllScreensWithIds(ScreenIds);
            var ParentScreens = new HashSet<ScreenMenuResponseDto>();
            foreach (var screen in response)
            {
                if (screen.ParentId != null)
                {
                    var parentScreen = await _unitOfWork.Screen.GetScreenById(screen.ParentId.Value);
                    ParentScreens.Add(new ScreenMenuResponseDto { ScreenId = parentScreen.Id, AreaName = parentScreen.AreaName, ScreenName = parentScreen.Name, ParentId = null, IsMenuScreen = parentScreen.IsMenuScreen });
                }
            }
            if (ParentScreens.Any())
            {
                response.AddRange(ParentScreens);
            }
            return response.Distinct().ToList();
        }
    }
}