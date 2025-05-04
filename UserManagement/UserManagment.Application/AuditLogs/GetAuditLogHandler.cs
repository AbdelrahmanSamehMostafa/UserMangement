using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.AuditLogs
{
    public record GetAuditLogDTO(LogRequestDto requestDto) : IRequest<AuditGetAllResult>;

    public class GetAuditLogHandler : IRequestHandler<GetAuditLogDTO, AuditGetAllResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAuditLogHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuditGetAllResult> Handle(GetAuditLogDTO request, CancellationToken cancellationToken)
        {
            if (request.requestDto.DateFrom > request.requestDto.DateTo)
            {
                throw new CustomException(ErrorResponseMessage.InvalidDate);
            }
            var (dateFrom, dateTo) = ResponseHelper.GetDateRange(request.requestDto.DateFrom, request.requestDto.DateTo);
            var updatedRequestDto = request.requestDto with { DateFrom = dateFrom, DateTo = dateTo };
            var _data = await _unitOfWork.AuditLog.GetAuditLogsByDateRangeAsync(updatedRequestDto, cancellationToken);
            
            if (!_data.ListOfLogs.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            var paginatedLogs = _data.ListOfLogs
                    .Skip((request.requestDto.PageNumber - 1) * request.requestDto.PageSize)
                    .Take(request.requestDto.PageSize)
                    .ToList();
            var logs = LogsMapping.AuditGetAllResult(paginatedLogs, _data.ListOfLogs.Count, request.requestDto.PageNumber, request.requestDto.PageSize, _data.Datefrom.Value, _data.DateTo.Value);

            return logs;
        }
    }
}