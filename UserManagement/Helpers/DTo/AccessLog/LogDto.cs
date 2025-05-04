namespace UserManagmentRazor.Helpers.DTo.AccessLog
{
    public class AccessLogDto
    {
        public string AccessType { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Status { get; set; }
        public string UserFullName { get; set; }
        public string EmailAddress { get; set; }
    }
}
