namespace UserManagment.Common.DTO.Common
{
    public record UserGroupsDTO
    {
        public Guid userId { get; init; }
        public List<Guid> groupIds { get; init; } = new List<Guid>();
    }
    public record GroupUserDTO
    {
        public Guid userId { get; init; }
        public Guid groupId { get; init; }
    }
}