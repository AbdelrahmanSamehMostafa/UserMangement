namespace UserManagmentRazor.Helpers.DTo.ConfigurationDTO
{
    public class UpdateEmailTemplateDto
    {
        public string ConfigKey { get; set; }
        public string ConfigType { get; set; }
        public string NewTemplateBody { get; set; }
    }
    public class UpdateEmailResponseDto
    {
        public string Message { get; set; }
    }
}
