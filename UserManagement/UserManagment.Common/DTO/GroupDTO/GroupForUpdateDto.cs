

namespace UserManagment.Common.DTO.GroupDTO
{
    public class GroupForUpdateDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public List<Guid> RolesIds { get; init; }
    }
}