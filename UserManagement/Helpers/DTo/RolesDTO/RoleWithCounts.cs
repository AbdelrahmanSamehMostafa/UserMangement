namespace com.gbg.modules.utility.Helpers.DTo.RolesDTO
{
    public class RoleWithCounts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CountOfUsers { get; set; } = 0;
        public int CountOfRoles { get; set; } = 0;
    }
}