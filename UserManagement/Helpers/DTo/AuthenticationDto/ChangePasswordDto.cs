namespace com.gbg.modules.utility.Helpers.DTo.AuthenticationDto
{
    public class ChangePasswordDto
    {
        public Guid userId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}