namespace com.gbg.modules.utility.Helpers.DTo.AuditLog
{
    public class AuditLog
    {
        public string logName { get; set; }
        public string EntityName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool Status { get; set; }
        public string UserFullName { get; set; }
        public string Email { get; set; }
    }
}