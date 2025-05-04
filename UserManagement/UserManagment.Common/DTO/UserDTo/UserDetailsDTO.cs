using UserManagment.Common.DTO.LookUps;

namespace UserManagment.Common.DTO.UserDTo
{
    public record UserDetailsDTO
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string MobileNumber { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string Location { get; init; }
        public string Username { get; init; }
        public IEnumerable<LookUpDTO>? Groups { get; set; }
        public byte[]? Image { get; set; }
    }
}