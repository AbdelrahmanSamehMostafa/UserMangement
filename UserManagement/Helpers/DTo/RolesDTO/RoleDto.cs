namespace UserManagmentRazor.Helpers.DTo.RolesDTO
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
