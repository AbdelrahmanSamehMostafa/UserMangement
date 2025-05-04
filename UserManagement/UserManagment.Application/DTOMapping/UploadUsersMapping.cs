namespace UserManagment.Application.DTOMapping
{
    public record UploadedUsersDTO
    {
        public List<UserResultDto> users { get; set; }
        public string details { get; set; }
        public int count { get; set; }
    }
    public static class UploadUsersMapping
    {
        public static UploadedUsersDTO ToUploadedUsersDTO(this (List<UserResultDto> users, string details, int count) result)
        {
            return new UploadedUsersDTO
            {
                users = result.users,
                details = result.details,
                count = result.count
            };
        }

    }
}