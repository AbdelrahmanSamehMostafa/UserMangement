namespace UserManagment.Common.DTO.UserDTo
{
    public record ChangePasswordDTO
    {
        public Guid userId { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }

    }
}