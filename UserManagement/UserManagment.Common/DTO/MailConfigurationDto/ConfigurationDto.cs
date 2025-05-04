namespace UserManagment.Common.DTO.MailConfigurationDto
{
    public record ConfigurationDTo
    {
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string ConfigType { get; set; }
    }
}