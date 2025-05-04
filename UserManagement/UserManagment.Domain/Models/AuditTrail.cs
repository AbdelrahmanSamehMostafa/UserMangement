using UserManagment.Common.Enum;

namespace UserManagment.Domain.Models
{
    public class AuditTrail:BaseModel
    {
        public Guid? UserId { get; set; }

        public User? User { get; set; }

        public LogsType LogsType { get; set; }
        
        public string? LogName { get; set; }

        public DateTime Date { get; set; }

        public required string EntityName { get; set; }

        public string? PrimaryKey { get; set; }

        public Dictionary<string, object?> OldValues { get; set; } = [];

        public Dictionary<string, object?> NewValues { get; set; } = [];

    }
}