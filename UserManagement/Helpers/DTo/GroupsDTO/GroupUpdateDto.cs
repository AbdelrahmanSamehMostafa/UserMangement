namespace com.gbg.modules.utility.Helpers.DTo.GroupsDTO
{
    public class GroupUpdateDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public List<Guid> RolesIds { get; set; } = new();
    }
}