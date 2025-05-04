using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.DTO.LogsDTO;

namespace UserManagment.Infrastructure.Repositories
{
    public class AuditLogRepository(UserManagmentDbContext ctx) : IAuditLogRepository
    {
        public async Task<(List<AuditListDTO> ListOfLogs, DateOnly? Datefrom, DateOnly? DateTo)> GetAuditLogsByDateRangeAsync(LogRequestDto requestDto, CancellationToken cancellationToken)
        {
            // Fetch logs between dateFrom and dateTo
            var logs = await ctx.AuditTrails.AsNoTracking()
                .Where(log => DateOnly.FromDateTime(log.InsertedDate.Value) >= requestDto.DateFrom && DateOnly.FromDateTime(log.InsertedDate.Value) <= requestDto.DateTo)
                .Join(ctx.Users,
                    log => log.UserId,
                    user => user.Id,
                    (log, user) => new
                    {
                        AuditLog = log,
                        User = user
                    })
                .ToListAsync(cancellationToken);

            var result = logs
                .Select(ti => new AuditListDTO
                (
                    ti.AuditLog.LogsType,

                    $"{ti.User?.FirstName ?? "Unknown"} {ti.User?.LastName ?? "User"}",
                    ti.User?.Email ?? "Unknown",
                    ti.AuditLog.InsertedDate.HasValue ? DateOnly.FromDateTime(ti.AuditLog.InsertedDate.Value) : (DateOnly?)null,
                    ti.AuditLog.InsertedDate.HasValue ? TimeOnly.FromDateTime(ti.AuditLog.InsertedDate.Value) : TimeOnly.MinValue,
                    ti.AuditLog.LogName ?? "Unknown",
                    ti.AuditLog.EntityName ?? "Unknown"
                ))
                .OrderByDescending(log => log.Date)
                .ToList();
            return (result, requestDto.DateFrom, requestDto.DateTo);
        }
    }
}