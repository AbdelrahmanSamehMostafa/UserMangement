using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Application.Commands;
using UserManagment.Application.Identity;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Application.Users;
using UserManagment.Common.DTO.Auth;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Enum;
using UserManagment.Common.Helpers;

namespace UserManagment.API.Users
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "User Management")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageCommand command)
        {
            _logger.LogInformation("In UserController, UploadImage API called for User ID: {UserId}", command.EntityId);

            return await ResponseHelper.HandleRequestAsync(
                command,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.ImageUploadedSuccessfully
            );
        }

        [RequiresPolicy("ListUser")]
        [HttpGet(Name = "GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] UserSearchInput ListingInput)
        {

            _logger.LogInformation("in UserController, GetUsers API called with search string: {ListingInput.SearchString}", ListingInput.SearchString);
            return await ResponseHelper.HandleRequestAsync(
                ListingInput,
                async (req) => await _mediator.Send(new DisplayUsers(req)),
                SuccessResponseMessage.RetrievedSuccessfully);
        }

        [RequiresPolicy("ExportUser")]
        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] UserSearchInput ListingInput)
        {
            _logger.LogInformation("in UserController, ExportToExcel API called");
            try
            {
                var (stream, contentType, fileName) = await _mediator.Send(new UserExportDTO(ListingInput));
                ResponseHelper.SetFileNameHeader(Response, fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "in UserController, Error in ExportToExcel");
                return BadRequest(ex.Message);
            }
        }
        [RequiresPolicy("ViewUser")]
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            _logger.LogInformation("in UserController, GetUserById API called ");
            return await ResponseHelper.HandleRequestAsync(
                id,
                async (requestId) => await _mediator.Send(new GetUserById(requestId)),
                SuccessResponseMessage.RetrievedSuccessfully);
        }

        [RequiresPolicy("ViewProfile")]
        [HttpGet("profile", Name = "GetUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                return Unauthorized("Authorization header missing");
            }

            var token = tokenHeader.ToString().Replace("Bearer ", "");
            var userId = TokenExtractor.GetClaimFromToken<Guid>(token, ClaimType.UserId);

            _logger.LogInformation("In UserController, GetUserProfile API called");

            return await ResponseHelper.HandleRequestAsync(
                userId,
                async (req) => await _mediator.Send(new GetUserById(req)),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }
        [RequiresPolicy("ResetUserPassword")]
        [HttpPut("password/Forget")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPasswordAsync([FromBody] NewPasswordDTO input)
        {
            _logger.LogInformation("In UserController, ForgetPasswordAsync API called with email: {Email}", input.Email);

            return await ResponseHelper.HandleRequestAsync(
                input,
                async (req) => await _mediator.Send(new ForgetPassword(input.Email)),
                SuccessResponseMessage.Success_ForgetPassword
            );
        }
        [RequiresPolicy("UpdateUser")]
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(UpdateUserCommand request)
        {

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully
            );
        }

        [RequiresPolicy("ChangePassword")]
        [HttpPut("ChangePassword", Name = "Change Password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO requestDto)
        {
            return await ResponseHelper.HandleRequestAsync(
                requestDto,
                async (req) => await _mediator.Send(new ChangePassword(req)),
                SuccessResponseMessage.Success_ChangePassword
            );
        }

        [RequiresPolicy("CreateUser")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(UserRequestDTO request)
        {
            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.CreationSuccessfully
            );
        }

        [HttpGet("DownloadErrorFile")]
        public IActionResult DownloadErrorFile([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

            if (!System.IO.File.Exists(tempFilePath))
            {
                return NotFound("The requested file does not exist.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
            System.IO.File.Delete(tempFilePath); // Optional: delete the file after downloading

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpPost]
        [Route("multipleCreate")]
        public async Task<IActionResult> UploadUsers([FromForm] UserUploadRequestDTO request)
        {
            var result = await _mediator.Send(request);
            switch (result)
            {
                case byte[] fileBytes:
                    // Set the file name for download
                    var fileName = "UploadErrors.xlsx";

                    // Save the file to the temp directory
                    string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
                    await System.IO.File.WriteAllBytesAsync(tempFilePath, fileBytes);

                    // Generate the download link using only the fileName
                    var errorResponse = new BaseResponse<string>
                    {
                        IsSuccess = false,
                        StatusCode = StatusResponsesCode.StatusCodeOK,
                        Message = "Upload failed. Please download the error file.",
                        Data = Url.Action("DownloadErrorFile", new { fileName }) // Download link for the error file
                    };

                    return Ok(errorResponse);

                case bool uploadedUsers:
                    var successResponse = new BaseResponse<bool>
                    {
                        IsSuccess = true,
                        StatusCode = StatusResponsesCode.StatusCodeOK,
                        Message = SuccessResponseMessage.UploadUsersSuccessfully,
                        Data = uploadedUsers
                    };
                    return Ok(successResponse);

                default:
                    throw new CustomException(ErrorResponseMessage.InternalServerError);
            }
        }

        [HttpPost("addGroups")]
        public async Task<IActionResult> AddGroupsAsync(UserGroupsDTO userGroupsDTO)
        {
            return await ResponseHelper.HandleRequestAsync(
                userGroupsDTO,
                async (req) => await _mediator.Send(new AddGroupsDto(req)),
                        SuccessResponseMessage.AddedSuccessfully
            );
        }
        [RequiresPolicy("DeleteUser")]
        [HttpDelete("{id}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            _logger.LogInformation("In UserController, DeleteUser API called");

            return await ResponseHelper.HandleRequestAsync(
                id,
                async (req) => await _mediator.Send(new DeleteUser(req)),
                SuccessResponseMessage.DeletionSuccessfully
            );
        }

    }
}