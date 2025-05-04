using UserManagment.Common.Enum;

namespace UserManagment.Common.DTO.Common
{
    public record ScreenActionDTO
    {
        public Guid ScreenId { get; set; }
        public string? ScreenName { get; set; }
        public IEnumerable<Actions> actions { get; set; }
    }

    public record Actions
    {
        public Guid Id { get; set; }
        public ActionType ActionType { get; set; }
        public string? ActionName { get; set; }
    }
}