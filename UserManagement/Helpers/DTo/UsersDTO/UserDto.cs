using com.gbg.modules.utility.Helpers.DTo.Common;

namespace com.gbg.modules.utility.Helpers.DTo.UsersDTO
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string? UserName { get; init; }

        public string Email { get; init; }

        public DateTime? PasswordLastUpdatedDate { get; init; }

        public string? MobileNumber { get; init; }

        public DateTime DateOfBirth { get; init; }

        public string Location { get; init; } = string.Empty;

        public List<Guid> GroupIds { get; set; } = new();

        public List<LookupDto> groups { get; set; } = new();

        public bool IsLocked { get; set; }

        public byte[]? Image { get; set; }
    }
}