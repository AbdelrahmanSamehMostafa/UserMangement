namespace UserManagmentRazor.Helpers.DTo.PermissionsDto
{
    internal class AddScreenActionsToRoleDto
    {
        public Guid roleId { get; set; }
        public List<Guid> screenActionIds { get; set; }
    }
}