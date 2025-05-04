using UserManagmentRazor.Helpers.DTo.Common;

namespace UserManagmentRazor.Helpers.DTo.AccessLog
{
    public class AccessLog : BaseListing
    {
        public List<AccessLogDto> accessLogs { get; set; }
        public string Message { get; set; }

    }
}
