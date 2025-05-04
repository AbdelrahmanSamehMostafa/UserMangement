using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Application.Commands;
using UserManagment.Application.Roles;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.DTO.RoleDTo;
using UserManagment.Common.Helpers;
namespace UserManagment.API.SystemRole
{

    [ApiController]
    [Route("/api/role")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Role")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IMediator mediator, ILogger<RoleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [RequiresPolicy("CreateRole")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(RoleRequestDTO request)
        {
            _logger.LogInformation("in RoleController , Create API called");

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.CreationSuccessfully);
        }

        [RequiresPolicy("UpdateRole")]
        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateRoleCommand request)
        {
            _logger.LogInformation("in RoleController , Update API called");

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully);
        }

        [RequiresPolicy("DeleteRole")]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid roleId)
        {
            _logger.LogInformation("in RoleController , delete API called");

            return await ResponseHelper.HandleRequestAsync(
                roleId,
                async (id) => await _mediator.Send(new DeleteRole(id)),
                SuccessResponseMessage.DeletionSuccessfully);
        }

        [RequiresPolicy("RoleList")]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] BaseListingInput request)
        {
            _logger.LogInformation("in RoleController , GetAll API called");

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(new RoleGetAll(req)),
                SuccessResponseMessage.RetrievedSuccessfully);
        }

        [RequiresPolicy("ExportRole")]
        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] BaseListingInput input)
        {
            _logger.LogInformation("in RoleController , ExportToExcel API called");
            try
            {
                var (stream, contentType, fileName) = await _mediator.Send(new RoleExportDTO(input));
                ResponseHelper.SetFileNameHeader(Response, fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "in RoleController, Error in ExportToExcel");
                return BadRequest(ex.Message);
            }
        }
        [RequiresPolicy("ViewRole")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("in RoleController , GetById API called");

            return await ResponseHelper.HandleRequestAsync(
                id,
                async (requestId) => await _mediator.Send(new RoleGetById(requestId)),
                SuccessResponseMessage.RetrievedSuccessfully);
        }

        [HttpPost("AddScreenActionsToRole")]
        public async Task<IActionResult> AddScreenActionsToRole(ScreenRolesDTO request)
        {
            _logger.LogInformation("in RoleController , AddScreenActionsToRole API called");

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(new ScreenRoleDTO(req)),
                SuccessResponseMessage.CreationSuccessfully);
        }

        [HttpGet("GetScreenActionsOfRole/{RoleId}")]
        public async Task<IActionResult> GetScreenActionsOfRole(Guid RoleId)
        {
            _logger.LogInformation("in RoleController , GetScreenActionsOfRole API called");
            return await ResponseHelper.HandleRequestAsync(
                RoleId,
                async (id) => await _mediator.Send(new ScreenRoleActionGet(id)),
                SuccessResponseMessage.RetrievedSuccessfully);
        }
    }
}
