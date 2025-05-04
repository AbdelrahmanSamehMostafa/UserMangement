
namespace UserManagment.Common.DTO.UserDTo
{
    public record UserForUpdateDTO
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string MobileNumber { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string Location { get; init; }
        public List<Guid> GroupIds { get; init; }
    }
}