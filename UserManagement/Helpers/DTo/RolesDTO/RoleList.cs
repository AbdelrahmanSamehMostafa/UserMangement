using UserManagmentRazor.Helpers.DTo.Common;

namespace UserManagmentRazor.Helpers.DTo.RolesDTO
{
    public class RoleList : BaseListing
    {
        public List<RoleDto> roles { get; set; }
    }
}
