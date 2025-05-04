using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Application.LookUps;
using UserManagment.Common.DTO.LookUps;
using UserManagment.Common.Enum;
using UserManagment.Common.Helpers;
namespace UserManagment.API.LookUp
{
    [ApiController]
    [Route("/api/LookUps")]
    [ApiExplorerSettings(GroupName = "LookUps")]
    [Authorize]
    public class LookUpController : ControllerBase

    {
        private readonly IMediator _mediator;
        private readonly ILogger<LookUpController> _logger;

        public LookUpController(IMediator mediator, ILogger<LookUpController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("getScreens")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("in YourController, GetAll API called");

            return await ResponseHelper.HandleRequestAsync(
                new ScreenActionsGetAllDTO(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [HttpGet]
        [Route("ScreenLookups")]
        public async Task<IActionResult> GetScreens()
        {
            _logger.LogInformation("in YourController, GetScreens API called");

            return await ResponseHelper.HandleRequestAsync(
                new ScreenGetAllDTO(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [HttpGet]
        [Route("ActionLookups")]
        public async Task<IActionResult> GetActions()
        {
            _logger.LogInformation("in YourController, GetActions API called");

            return await ResponseHelper.HandleRequestAsync(
                new object(),
                async (_) =>
                {
                    var actions = Enum.GetValues(typeof(ActionType))
                                      .Cast<ActionType>()
                                      .Select(a => new IntIdLookupDTO { Id = (int)a, Name = a.ToString() })
                                      .ToList();
                    return actions;
                },
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [HttpGet]
        [Route("RoleLookUps")]
        public async Task<IActionResult> GetRoles()
        {
            _logger.LogInformation("in YourController, GetRoles API called");

            return await ResponseHelper.HandleRequestAsync(
                new RoleLookUpDTO(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }


    }
}