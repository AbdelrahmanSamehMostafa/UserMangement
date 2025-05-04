

namespace UserManagment.Common.DTO.LogsDTO
{
    public record AuditSettings
    {
        public List<string> TablesToSkip { get; set; } = new List<string>();

    }
}