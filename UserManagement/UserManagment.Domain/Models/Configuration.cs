using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UserManagment.Domain.Models
{
    [Index(nameof(ConfigKey), AllDescending = true)]
    public partial class Configuration : BaseModel
    {
        public static string MaxTrial_TYPE = "MaxTrial";
        public static string MaxTrial_Key_MaxTrial = "MaxTrial";
        public static string MaxTrial_Key_MaxDurationInMinutes = "MaxDurationInMinutes";



        public static string PasswordExpirationPeriod_Type = "PasswordExpirationPeriod";

        public static string PasswordExpirationPeriod_Key = "PasswordExpirationPeriod";

        public static string DomainFormat_Type = "DomainFormat_Type";

        public static string DomainFormat_Key = "DomainFormat";



        public static string EMAIL_TYPE = "email";
        public static string EMAIL_TEMPLATE_TYPE = "emailtemplate";

        public static string SMPTHOST = "smtp.host";

        public static string SMPTPORT = "smtp.port";

        public static string SMPTUSERNAME = "smtp.username";

        public static string SMPTPASSWORD = "smtp.password";

        public static string SMPTISSSL = "smtp.isssl";

        public static string SMPTDISPLAYNAME = "smtp.displayname";

        public static string WELCOME_EMAIL_KEY = "WelcomeEmail";

        public static string CHANGE_PASSWORD_KEY = "ChangePassword";
        
        public static string RESET_PASSWORD_KEY = "ResetPassword";


        [Key]
        public required string ConfigKey { get; set; }
        public required string ConfigValue { get; set; }
        public required string ConfigType { get; set; }
    }

}
