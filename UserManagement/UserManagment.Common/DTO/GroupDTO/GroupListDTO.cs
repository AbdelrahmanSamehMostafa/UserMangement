

namespace UserManagment.Common.DTO.GroupDTO
{
    public record GroupListDTO
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Code { get; init; }
        public string? Description { get; init; }
        public int CountOfUsers { get; init; }
        public int CountOfRoles { get; init; }
    }
}