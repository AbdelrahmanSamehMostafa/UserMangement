namespace com.gbg.modules.utility.Helpers.DTo.UsersDTO
{
    public class UserToUpdateDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? UserName { get; set; }

        public string Email { get; set; }

        public DateTime? PasswordLastUpdatedDate { get; set; }

        public string? MobileNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Location { get; set; } = string.Empty;

        public List<Guid> GroupIds { get; set; } = new();
    }
}