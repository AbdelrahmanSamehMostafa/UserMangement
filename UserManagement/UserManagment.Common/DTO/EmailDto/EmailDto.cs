namespace UserManagment.Common.DTO.EmailDto
{
    public class EmailDto
    {
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}