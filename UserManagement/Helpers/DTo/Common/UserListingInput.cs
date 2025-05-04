using UserManagmentRazor.Helpers.Enums;

namespace UserManagmentRazor.Helpers.DTo.Common
{
    public record UserListingInput : BaseListingInput
    {
        public LockStatus LockStatus { get; set; } = LockStatus.All;
    }
}
