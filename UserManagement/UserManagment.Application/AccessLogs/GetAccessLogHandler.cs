using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.Helpers;
using Abp.Extensions;

namespace UserManagment.Application.AccessLogs
{
    public record GetAccessLogsQuery(LogRequestDto requestDto) : IRequest<AccessGetAllResult>;

    public class GetAccessLogsHandler : IRequestHandler<GetAccessLogsQuery, AccessGetAllResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAccessLogsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AccessGetAllResult> Handle(GetAccessLogsQuery request, CancellationToken cancellationToken)
        {
            if (request.requestDto.DateFrom > request.requestDto.DateTo)
            {
                throw new CustomException(ErrorResponseMessage.InvalidDate);
            }
            var (dateFrom, dateTo) = ResponseHelper.GetDateRange(request.requestDto.DateFrom, request.requestDto.DateTo);
            var updatedRequestDto = request.requestDto with { DateFrom = dateFrom, DateTo = dateTo };
            var _data = await _unitOfWork.AccessLog.GetAccessLogsByDateRangeAsync(updatedRequestDto, cancellationToken);
            
            if(!_data.ListOfLogs.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            var paginatedLogs = _data.ListOfLogs
                    .Skip((request.requestDto.PageNumber - 1) * request.requestDto.PageSize)
                    .Take(request.requestDto.PageSize)
                    .ToList();
            var logs = LogsMapping.AccessGetAllResult(paginatedLogs, _data.ListOfLogs.Count, request.requestDto.PageNumber, request.requestDto.PageSize, _data.Datefrom.Value, _data.DateTo.Value);
            return logs;
        }
    }
}
