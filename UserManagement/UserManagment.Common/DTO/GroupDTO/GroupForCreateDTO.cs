

namespace UserManagment.Common.DTO.GroupDTO
{
    public record GroupForCreateDTO
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<Guid> RolesIds { get; set; }

    }
}