using MediatR;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.AccessLogs
{
    public record ExportAccessLogDTO(LogRequestDto requestDto) : IRequest<(MemoryStream, string, string)>;

    public class ExportAccessLogHandler : IRequestHandler<ExportAccessLogDTO, (MemoryStream, string, string)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExportAccessLogHandler> _logger;

        public ExportAccessLogHandler(IUnitOfWork unitOfWork, ILogger<ExportAccessLogHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<(MemoryStream, string, string)> Handle(ExportAccessLogDTO request, CancellationToken cancellationToken)
        {
            if (request.requestDto.DateFrom > request.requestDto.DateTo)
            {
                throw new CustomException(ErrorResponseMessage.InvalidDate);
            }
            var (dateFrom, dateTo) = ResponseHelper.GetDateRange(request.requestDto.DateFrom, request.requestDto.DateTo);
            var updatedRequestDto = request.requestDto with { DateFrom = dateFrom, DateTo = dateTo };
            var logs = await _unitOfWork.AccessLog.GetAccessLogsByDateRangeAsync(updatedRequestDto, cancellationToken);

            if (!logs.ListOfLogs.Any())
            {
                _logger.LogWarning("No access logs found for export");
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            var memoryStream = await ExportExcel.GenerateExcelAsync(logs.ListOfLogs, ExportExeclConfig.AccessLogConfig.Columns, ExportExeclConfig.AccessLogConfig.Properties,
                        ExportExeclConfig.AccessLogConfig.ReportFileName, _logger);
            var (contentType, fileName) = ResponseHelper.GetExcelFileDetails("AuditLogs");
            return (memoryStream, contentType, fileName);
        }
    }
}