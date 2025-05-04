using UserManagment.Common.DTO.Common;
using UserManagment.Common.Enum;

namespace UserManagment.Common.DTO.UserDTo
{
    public record UserSearchInput : BaseListingInput
    {
        public LockStatus LockStatus { get; set; }
    }
}