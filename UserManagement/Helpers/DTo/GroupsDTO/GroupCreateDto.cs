namespace UserManagmentRazor.Helpers.DTo.GroupsDTO
{
    public class GroupCreateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
    }
}
