using com.gbg.modules.utility.Helpers.DTo.Common;
using com.gbg.modules.utility.Helpers.DTo.UsersDTO;

namespace UserManagmentRazor.Helpers.DTo.GroupsDTO
{
    public class GroupResponseDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<Guid> RolesIds { get; set; } = new();
        public List<LookupDto> roles { get; set; } = new();
        public List<UserLookupDto> users { get; set; } = new();
    }
}
