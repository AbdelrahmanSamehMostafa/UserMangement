
namespace UserManagment.Common.DTO.UserDTo
{
    public record UserListDto : UserBaseListDto
    {
        public DateOnly DateOfBirth { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Groups { get; set; }
    }
}