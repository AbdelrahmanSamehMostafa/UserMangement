

namespace UserManagment.Common.DTO.GroupUserDTO
{
    public record UserGroupsDTO
    {
        public Guid UserId { get; set; }
        public string Groups { get; set; }
        
    }
}