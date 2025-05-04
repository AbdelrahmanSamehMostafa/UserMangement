using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.Enum;
using UserManagment.Application.ScreenMenu;
using UserManagment.Common.Helpers;

namespace UserManagment.API.ScreenMenus
{
    [ApiController]
    [Route("api/screen")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Screen Menu")]
    public class MainMenuController : Controller
    {
        private readonly ILogger<MainMenuController> _logger;
        private readonly IMediator _mediator;

        public MainMenuController(ILogger<MainMenuController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetScreenPermissions/{ScreenId}")]
        public async Task<IActionResult> GetScreenPermissions(Guid ScreenId)
        {
            _logger.LogInformation("in YourController, GetScreenPermissions API called with ScreenId: {ScreenId}", ScreenId);

            if (!Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                return Unauthorized("Authorization header missing");
            }

            var token = tokenHeader.ToString().Replace("Bearer ", "");
            var userId = TokenExtractor.GetClaimFromToken<Guid>(token, ClaimType.UserId);

            return await ResponseHelper.HandleRequestAsync(
                new ScreenPermissionRequest(ScreenId, userId),
                async (request) => await _mediator.Send(request),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [HttpGet("MainMenus")]
        public async Task<IActionResult> GetMainMenus()
        {
            _logger.LogInformation("in YourController, GetMainMenus API called");

            if (!Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                return Unauthorized("Authorization header missing");
            }

            var token = tokenHeader.ToString().Replace("Bearer ", "");
            var userId = TokenExtractor.GetClaimFromToken<Guid>(token, ClaimType.UserId);

            return await ResponseHelper.HandleRequestAsync(
                new GetMainMenusById(userId), 
                async (request) => await _mediator.Send(request), 
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }
    }
}