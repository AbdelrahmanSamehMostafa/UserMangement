using UserManagmentRazor.Helpers.DTo.Common;

namespace UserManagmentRazor.Helpers.DTo.GroupsDTO
{
    public class GroupList : BaseListing
    {
        public List<GroupWithCounts> Groups { get; set; }
    }
}
