namespace UserManagment.Common.DTO.RoleDTo
{
    public record ScreenRolesDTO
    {
        public Guid RoleId { get; init; }
        public List<Guid> ScreenActionIds { get; set; }
    }

}