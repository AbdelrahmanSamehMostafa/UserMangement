using UserManagment.Common.DTO.LogsDTO;
using UserManagment.Common.Enum;
using UserManagment.Domain.Models;

namespace UserManagment.Application.DTOMapping
{
    public record AccessLogRequest
    (AccessStatus AccessStatus, Guid userId);
    public record AccessGetAllResult
    {
        public IEnumerable<AccessLogResponseDto> AccessLogs { get; set; }
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
    public record AuditGetAllResult
    {
        public IEnumerable<AuditListDTO> AuditLogs { get; set; }
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }


    public static class LogsMapping
    {
        public static AccessLog ToAccessLog(this AccessLogRequest row)
        {
            var res = new AccessLog
            {
                UserId = row.userId,
                AccessStatus = row.AccessStatus,
                InsertedDate = DateTime.UtcNow
            };
            return res;
        }
        public static AccessGetAllResult AccessGetAllResult(IEnumerable<AccessLogResponseDto> accessLogs,
        int count, int pageNumber, int pageSize, DateOnly dateFrom, DateOnly dateTo)
        {
            return new AccessGetAllResult
            {
                AccessLogs = accessLogs.ToList(),
                Count = count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }
        public static AuditGetAllResult AuditGetAllResult(IEnumerable<AuditListDTO> auditLogs,
        int count, int pageNumber, int pageSize, DateOnly dateFrom, DateOnly dateTo)
        {
            return new AuditGetAllResult
            {
                AuditLogs = auditLogs.ToList(),
                Count = count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }

    }
    public record LogRequestDto(DateOnly? DateFrom, DateOnly? DateTo, int PageNumber = 1, int PageSize = 20);
    public record AccessLogResponseDto
    (
        string AccessType,
        DateOnly Date,
        TimeOnly Time,
        string Status,
        string UserFullName,
        string EmailAddress
    );
}
