namespace UserManagment.Common.Helpers
{
    public static class SuccessResponseMessage
    {
        // Success Messages for API Responses
        public const string RetrievedSuccessfully = "Retrieved Successfully!";
        public const string CreationSuccessfully = "Created Successfully!";
        public const string AddedSuccessfully = "Added Successfully!";
        public const string UpdationSuccessfully = "Updated Successfully!";
        public const string DeletionSuccessfully = "Deleted Successfully!";
        public const string UploadUsersSuccessfully = "Users have been successfully uploaded and processed";
        public const string ImageUploadedSuccessfully = "Image uploaded successfully!";

        // Authentication Success Messages
        public const string Success_login = "Login Successfully!";
        public const string Success_logout = "Logout Successfully!";
        public const string Success_ResetPassword = "Reset password mail sent successfully.";
        public const string Success_ForgetPassword = "Forget password mail sent successfully.";
        public const string Success_ChangePassword = "Password changed successfully.";

        // Other Success Messages
        public const string Success_Upload = "Uploaded Successfully!";
        public const string UnlockUser = "User unlocked successfully!";
        public const string LockUser = "User locked successfully!";
    }
}