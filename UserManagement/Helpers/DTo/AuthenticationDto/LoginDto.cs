using System.ComponentModel.DataAnnotations;

namespace com.gbg.modules.utility.Helpers.DTo.AuthenticationDto
{
    public class LoginDto
    {
        [EmailAddress]
        public required string emailAddress { get; set; }

        [DataType(DataType.Password)]
        public required string password { get; set; }
    }
}