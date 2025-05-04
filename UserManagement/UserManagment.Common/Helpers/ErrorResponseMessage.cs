namespace UserManagment.Common.Helpers
{
    public static class ErrorResponseMessage
    {
        // General Error Messages
        public const string Unauthorized = "Unauthorized!";
        public const string NotFound = "Not Found!";
        public const string InternalServerError = "Internal Server Error!";
        public const string NullRequest = "Request cannot be null!";

        // Data Validation Errors
        public const string isRepeated = "Data already exists!";
        public const string InvalidDate = "Date From cannot be greater than Date To.";
        public const string Negative_Values = "Negative values are not allowed!";
        public const string DomainFormatRequired = "Domain format is required!";

        // User and Authentication Errors
        public const string Forbidden = "You Do not Have Permission to Access this Resource!";
        public const string LogoutFailed = "Logout Failed!";
        public const string User_Locked = "User is locked!";
        public const string User_MailDomain = "Invalid Email Domain!";
        public const string User_NotFound = "User not found!";
        public const string UploadUsersError = "Some users couldn't be uploaded. Please check the uploaded file for details";
        public const string User_Invalidcredentials = "Invalid Email OR Password!";
        public const string User_InvalidOldPassword = "Invalid Old Password!";
        public const string UserImageAlreadyExists = "User Image already exists!";

        // Login Specific Errors
        public const string Login_AccountNotActive = "User account was not active.";
        public const string Login_AccountLocked = "User account was locked.";
        public const string Login_UserNameOrPasswordNotValid = "Username or password was not valid.";
        public const string Login_PasswordExpired = "Password expired; you have to change your password.";

        // Group and Role Errors
        public const string Group_NotFound = "Group not found!";
        public const string RolesUser_NotFound = "No roles found for the user!";
        public const string Role_Default = "Default role already exists!";
        public const string DefaultRole = "Default role Can not be Deleted!";
        public const string RoleScreen_NotFound = "No screens found for the user's roles!";
        public const string NoRoles = "At least one role is required!";
        public const string NoGroups = "At least one group is required!";

        // Email and Configuration Errors
        public const string SendEmailRequest = "Send email request is null!";
        public const string SendEmailFailed = "Email configuration is incomplete.";
        public const string EmailTemplate_NotFound = "Email template not found!";
        public const string InvalidConfigKey = "Invalid configuration key!";

        // Change Password Errors
        public const string ChangePasswordFailed = "Change password failed!";
    }
}
