using UserManagment.Application.DTOMapping;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IAccessLogRepository
    {
        Task<Guid?> Create(AccessLogRequest request);
        Task<(List<AccessLogResponseDto> ListOfLogs, DateOnly? Datefrom, DateOnly? DateTo)> GetAccessLogsByDateRangeAsync(LogRequestDto requestDto, CancellationToken cancellationToken);


    }
}
