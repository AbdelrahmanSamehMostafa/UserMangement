namespace com.gbg.modules.utility.Helpers.DTo.AuthenticationDto
{
    public class LoginResponseDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string AvatarURL { get; set; }
        public bool IsSuccess { get; set; }
    }
}