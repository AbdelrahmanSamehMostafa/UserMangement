

namespace UserManagment.Common.DTO.GroupUserDTO
{
    public record GroupUsersCountDTO
    {
        public Guid GroupId { get; set; }
        public int UsersCount { get; set; }
    }
}