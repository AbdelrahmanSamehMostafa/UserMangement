

namespace UserManagment.Common.DTO.GroupRole
{
    public record GroupRolesCountDTO
    {
        public Guid GroupId { get; set; }
        public int RolesCount { get; set; }
    }
}