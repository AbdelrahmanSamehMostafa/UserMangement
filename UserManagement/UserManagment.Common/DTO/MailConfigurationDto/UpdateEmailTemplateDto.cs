
namespace UserManagment.Common.DTO.ConfigurationDto
{
    public record UpdateEmailTemplateDto
    {
        public string ConfigKey { get; set; }
        public string ConfigType { get; set; }
        public string NewTemplateBody { get; set; }
    }
}