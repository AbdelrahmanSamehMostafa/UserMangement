using com.gbg.modules.utility.Helpers.DTo.Common;

namespace com.gbg.modules.utility.Helpers.DTo.UsersDTO
{
    public class UserViewDto
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

        public List<LookupDto> Groups { get; set; } = new();

        public byte[] Image { get; set; }
    }
}