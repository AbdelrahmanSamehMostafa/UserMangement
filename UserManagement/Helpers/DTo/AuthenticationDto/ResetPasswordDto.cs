using System.ComponentModel.DataAnnotations;

namespace com.gbg.modules.utility.Helpers.DTo.AuthenticationDto
{
    public class ResetPasswordDto
    {
        [EmailAddress]
        public required string Email { get; set; }
    }
}