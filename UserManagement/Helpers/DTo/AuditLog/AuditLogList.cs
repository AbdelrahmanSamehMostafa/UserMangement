using UserManagmentRazor.Helpers.DTo.Common;

namespace com.gbg.modules.utility.Helpers.DTo.AuditLog
{
    public class AuditLogList : BaseListing
    {
        public List<AuditLog> auditLogs { get; set; }
        public string Message { get; set; }
    }
}