using UserManagmentRazor.Helpers.DTo.Common;

namespace com.gbg.modules.utility.Helpers.DTo.UsersDTO
{
    public class UserList : BaseListing
    {
        public List<UserDto> users { get; set; } = new List<UserDto>();
    }
}