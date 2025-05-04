using Microsoft.EntityFrameworkCore;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.DTOMapping;
using UserManagment.Common.Enum;

namespace UserManagment.Infrastructure.Repositories
{
    public class AccessLogRepository(UserManagmentDbContext ctx) : IAccessLogRepository
    {
        public async Task<Guid?> Create(AccessLogRequest request)
        {
            var res = await ctx.AccessLogs.AddAsync(LogsMapping.ToAccessLog(request));
            return res.Entity.Id;
        }

        public async Task<(List<AccessLogResponseDto> ListOfLogs, DateOnly? Datefrom, DateOnly? DateTo)> GetAccessLogsByDateRangeAsync(LogRequestDto request, CancellationToken cancellationToken)
        {
            // Fetch logs between dateFrom and dateTo
            var logs = await ctx.AccessLogs.AsNoTracking()
                .Where(log => DateOnly.FromDateTime(log.InsertedDate.Value) >= request.DateFrom && DateOnly.FromDateTime(log.InsertedDate.Value) <= request.DateTo)
                .Join(ctx.Users,
                    log => log.UserId,
                    user => user.Id,
                    (log, user) => new
                    {
                        AccessLog = log,
                        User = user
                    })
                .ToListAsync(cancellationToken);

            // Perform the projection to AccessLogResponseDto and apply ordering on the client side
            var result = logs
                .Where(ti => !ti.User.IsDeleted)
                .Select(ti => new AccessLogResponseDto
                (
                    ti.AccessLog.AccessStatus == AccessStatus.LogOutSuccess ? "Logout" : "Login",
                    DateOnly.FromDateTime(ti.AccessLog.InsertedDate.Value),
                    TimeOnly.FromDateTime(ti.AccessLog.InsertedDate.Value),
                    ti.AccessLog.AccessStatus.ToString(),
                    $"{ti.User.FirstName} {ti.User.LastName}",
                    ti.User.Email
                ))
                .OrderByDescending(log => log.Date)  // Apply ordering on the projected data
                .ToList();

            return (result, request.DateFrom, request.DateTo);
        }

    }
}
