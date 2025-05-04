namespace UserManagment.Common.DTO.Common
{
    public record GroupRoleDTO
    {
        public Guid GroupId { get; init; }
        public List<Guid> RolesId { get; init; } = new List<Guid>();
    }
}