namespace UserManagmentRazor.Helpers.DTo.GroupsDTO
{
    public class GroupWithCounts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int CountOfUsers { get; set; } = 0;
        public int CountOfRoles { get; set; } = 0;
    }
}
