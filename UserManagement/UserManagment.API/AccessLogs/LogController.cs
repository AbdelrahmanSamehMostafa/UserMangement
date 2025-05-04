using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Application.AccessLogs;
using UserManagment.Application.AuditLogs;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.Helpers;

namespace UserManagment.API.Logs
{
    [ApiController]
    [Route("api/Logs")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Logs")]
    public class LogController : Controller
    {
        private readonly ILogger<LogController> _logger;
        private readonly IMediator _mediator;
        public LogController(IMediator mediator, ILogger<LogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [RequiresPolicy("ViewAcessLog")]
        [HttpGet("Accesslogs", Name = "GetAccessLogs")]
        public async Task<IActionResult> GetAccessLogs([FromQuery] LogRequestDto requestDto)
        {
            _logger.LogInformation("in LogController, GetAccessLogs API called");
            return await ResponseHelper.HandleRequestAsync(
                requestDto,
                async (req) => await _mediator.Send(new GetAccessLogsQuery(req)),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [RequiresPolicy("AuditLogsView")]
        [HttpGet("Auditlogs", Name = "GetAuditLogs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] LogRequestDto requestDto)
        {
            _logger.LogInformation("in LogController, GetAuditLogs API called");
            return await ResponseHelper.HandleRequestAsync(
                requestDto,
                async (req) => await _mediator.Send(new GetAuditLogDTO(req)),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [RequiresPolicy("ExportAccessLog")]
        [HttpGet("ExportAccessToExcel")]
        public async Task<IActionResult> ExportAccessToExcel([FromQuery] LogRequestDto requestDto)
        {
            _logger.LogInformation("in LogController , ExportToExcel API called");
            try
            {
                var (stream, contentType, fileName) = await _mediator.Send(new ExportAccessLogDTO(requestDto));
                ResponseHelper.SetFileNameHeader(Response, fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "in LogController, Error in ExportToExcel");
                return BadRequest(ex.Message);
            }
        }

        [RequiresPolicy("ExportAuditLogs")]
        [HttpGet("ExportAuditToExcel")]
        public async Task<IActionResult> ExportAuditToExcelxcel([FromQuery] LogRequestDto requestDto)
        {
            _logger.LogInformation("in LogController , ExportToExcel API called");
            try
            {
                var (stream, contentType, fileName) = await _mediator.Send(new ExportAuditLogDTO(requestDto));
                ResponseHelper.SetFileNameHeader(Response, fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "in LogController, Error in ExportToExcel");
                return BadRequest(ex.Message);
            }
        }
    }
}