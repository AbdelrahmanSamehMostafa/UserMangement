using MediatR;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.AuditLogs
{
    public record ExportAuditLogDTO(LogRequestDto requestDto) : IRequest<(MemoryStream, string, string)>;
    public class ExportAuditLogHandler : IRequestHandler<ExportAuditLogDTO, (MemoryStream, string, string)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExportAuditLogHandler> _logger;
        public ExportAuditLogHandler(IUnitOfWork unitOfWork, ILogger<ExportAuditLogHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<(MemoryStream, string, string)> Handle(ExportAuditLogDTO request, CancellationToken cancellationToken)
        {
            if (request.requestDto.DateFrom > request.requestDto.DateTo)
            {
                throw new CustomException(ErrorResponseMessage.InvalidDate);
            }
            var (dateFrom, dateTo) = ResponseHelper.GetDateRange(request.requestDto.DateFrom, request.requestDto.DateTo);
            var updatedRequestDto = request.requestDto with { DateFrom = dateFrom, DateTo = dateTo };
            var logs = await _unitOfWork.AuditLog.GetAuditLogsByDateRangeAsync(updatedRequestDto, cancellationToken);

            if (!logs.ListOfLogs.Any())
            {
                _logger.LogWarning("No audit logs found for export");
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            var memoryStream = await ExportExcel.GenerateExcelAsync(logs.ListOfLogs, ExportExeclConfig.AuditLogConfig.Columns, ExportExeclConfig.AuditLogConfig.Properties,
                        ExportExeclConfig.AuditLogConfig.ReportFileName, _logger);
            var (contentType, fileName) = ResponseHelper.GetExcelFileDetails("AuditLogs");
            return (memoryStream, contentType, fileName);
        }
    }
}