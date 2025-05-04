
using UserManagment.Common.Enum;

namespace UserManagment.Common.DTO.LogsDTO
{
    public record AuditListDTO
    {
        public LogsType logsType { get; set; }
        public string? logName { get; set; }
        public string? UserFullName { get; set; }
        public string? Email { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly time { get; set; }
        public bool Status { get; set; }=true;
        public string? EntityName { get; set; }

        public AuditListDTO(LogsType logsType, string? userFullName, string? email, DateOnly? date, TimeOnly time,string logName,string EntityName)
        {
            this.logsType = logsType;
            this.UserFullName = userFullName;
            this.Email = email;
            this.Date = date;
            this.time = time;
            this.logName = logName;
            this.EntityName = EntityName;
        }
    }
}