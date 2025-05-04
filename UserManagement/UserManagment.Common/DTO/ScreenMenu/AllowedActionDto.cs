using UserManagment.Common.Enum;

namespace UserManagment.Common.DTO.ScreenMenu
{
    public record AllowedActionDto
    {
        public Guid ActionId { get; set; }
        public string ActionName { get; set; }
        public ActionType ActionType { get; set; }
    }
}
