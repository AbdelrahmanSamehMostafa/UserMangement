using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Common.DTO.SearchInputs;
using UserManagment.Application.Group;
using UserManagment.Common.DTO.GroupDTO;
using Microsoft.AspNetCore.Authorization;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.Helpers;

namespace UserManagment.API.group
{
    [ApiController]
    [Route("/api/group")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Group")]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IMediator mediator, ILogger<GroupController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [RequiresPolicy("ExportGroup")]
        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] GroupInputSearch input)
        {
            _logger.LogInformation("in GroupController , ExportToExcel API called");
            try
            {
                var (stream, contentType, fileName) = await _mediator.Send(new GroupExportDTO(input));
                ResponseHelper.SetFileNameHeader(Response, fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "in GroupController, Error in ExportToExcel");
                return BadRequest(ex.Message);
            }
        }

        [RequiresPolicy("UpdateGroup")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupForUpdateDto groupDTO)
        {
            _logger.LogInformation("in GroupController,UpdateGroup API called");

            return await ResponseHelper.HandleRequestAsync(
                 groupDTO,
                 async (req) => await _mediator.Send(new GroupUpdateDTO(req)),
                 SuccessResponseMessage.UpdationSuccessfully);
        }

        [RequiresPolicy("GroupList")]
        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroups([FromQuery] GroupInputSearch baseListingInput)
        {
            _logger.LogInformation("in GroupController,GetGroups API called");
            return await ResponseHelper.HandleRequestAsync(
                baseListingInput,
                async (req) => await _mediator.Send(new GroupGetAllDTO(req)),
                SuccessResponseMessage.RetrievedSuccessfully);
        }
        [RequiresPolicy("GroupDetails")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            _logger.LogInformation("in GroupController,GetGroupById API called");
            return await ResponseHelper.HandleRequestAsync(
              id,
              async (requestId) => await _mediator.Send(new GroupGetByIdDTO(requestId)),
              SuccessResponseMessage.RetrievedSuccessfully);
        }
        [RequiresPolicy("CreateGroup")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(GroupCreateDTO group)
        {
            _logger.LogInformation("in GroupController, Create Group API called");
            return await ResponseHelper.HandleRequestAsync(
                group,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.CreationSuccessfully);
        }
        
        [RequiresPolicy("DeleteGroup")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            _logger.LogInformation("in GroupController, DeleteGroup API called");
            return await ResponseHelper.HandleRequestAsync(
                id,
                async (requestId) => await _mediator.Send(new GroupDeleteByIdDTO(requestId)),
                SuccessResponseMessage.DeletionSuccessfully);
        }

        [HttpPost("AddUserToGroup")]
        public async Task<IActionResult> AddUserAsync(GroupUserDTO groupUserDTO)
        {
            _logger.LogInformation("in GroupController, AddUserAsync API called");
            return await ResponseHelper.HandleRequestAsync(
                groupUserDTO,
                async (req) =>
                 await _mediator.Send(new AddUserDto(req)),
                SuccessResponseMessage.AddedSuccessfully);
        }

        [HttpPost("AddRolesToGroup")]
        public async Task<IActionResult> AddRolesAsync(GroupRoleDTO groupRoleDTO)
        {
            _logger.LogInformation("in GroupController, AddRolesAsync API called");
            return await ResponseHelper.HandleRequestAsync(
                groupRoleDTO,
                async (req) =>
                 await _mediator.Send(new AddGroupRoleDto(req)), SuccessResponseMessage.AddedSuccessfully);
        }
    }
}