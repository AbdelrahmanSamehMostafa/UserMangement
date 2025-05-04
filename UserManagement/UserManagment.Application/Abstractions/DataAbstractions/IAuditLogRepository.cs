using UserManagment.Application.DTOMapping;
using UserManagment.Common.DTO.LogsDTO;

namespace UserManagment.Application.Abstractions
{
    public interface IAuditLogRepository
    {
        Task<(List<AuditListDTO> ListOfLogs, DateOnly? Datefrom, DateOnly? DateTo)> GetAuditLogsByDateRangeAsync(LogRequestDto requestDto, CancellationToken cancellationToken);
    }
}