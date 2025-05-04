namespace UserManagment.Common.DTO.UserDTo
{
    public record UserBaseListDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string MobileNumber { get; set; }
        public bool IsLocked { get; set; }
        public byte[]? Image { get; set; }
    }
}