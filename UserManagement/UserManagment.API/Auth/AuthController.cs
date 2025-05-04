using UserManagment.Application.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static UserManagment.Application.Identity.LoginForDevelopmentHandler;
using UserManagment.Common.DTO.Auth;
using UserManagment.Common.Helpers;

namespace UserManagment.API.Auth
{
    [ApiController]
    [Route("/api/auth")]
    [ApiExplorerSettings(GroupName = "Authentication And Authorization")]

    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login request)
        {
            _logger.LogInformation("in AuthController, Login API called");

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.Success_login
            );
        }


#if DEBUG
        [HttpPost("Get-Token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenForDevelopment()
        {
            _logger.LogInformation("in AuthenticationController, GetTokenForDevelopment API called");

            LoginForDev input = new LoginForDev(
                EmailAddress: "admin@gmail.com",
                Password: "admin"
            );

            return await ResponseHelper.HandleRequestAsync(
                input,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.Success_login
            );
        }
#endif

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("in AuthController, Logout API called");

            return await ResponseHelper.HandleRequestAsync(
                new LogoutCommand(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.Success_logout,
                ErrorResponseMessage.LogoutFailed
            );
        }
        [RequiresPolicy("ResetPassword")]
        [HttpPut("password/reset")]
        [Authorize]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] NewPasswordDTO input)
        {
            _logger.LogInformation("in AuthenticationController, ResetPasswordAsync API called with email: {Email}", input.Email);

            return await ResponseHelper.HandleRequestAsync(
                input,
                async (req) => await _mediator.Send(new ResetPassword(input.Email)),
               SuccessResponseMessage.Success_ResetPassword
            );
        }
        [RequiresPolicy("UnlockUser")]
        [HttpPut("User/Unlock/{id}")]
        [Authorize]
        public async Task<IActionResult> UnlockUser([FromRoute] Guid id)
        {
            _logger.LogInformation("in AuthenticationController, UnlockUser API called with userId: {id}", id);

            return await ResponseHelper.HandleRequestAsync(
                id,
                async (userId) => await _mediator.Send(new UnlockUserCommand(userId)),
                SuccessResponseMessage.UnlockUser
            );
        }

        [RequiresPolicy("LockUser")]
        [HttpPut("User/lock/{id}")]
        [Authorize]
        public async Task<IActionResult> LockUser([FromRoute] Guid id)
        {
            _logger.LogInformation("in AuthenticationController, LockUser API called with userId: {id}", id);

            return await ResponseHelper.HandleRequestAsync(
                id,
                async (userId) => await _mediator.Send(new LockUserCommand(userId)),
               SuccessResponseMessage.LockUser
            );
        }
    }
}
