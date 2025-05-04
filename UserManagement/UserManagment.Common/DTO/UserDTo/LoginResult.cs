namespace UserManagment.Common.DTO.UserRecords
{

    public record LoginResult
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid? AvatarId { get; set; }
        public string AvatarURL { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }
}
