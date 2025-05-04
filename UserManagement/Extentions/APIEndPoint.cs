namespace UserManagmentRazor.Extentions
{
    public static class APIEndPoint
    {
        public static string Login = "auth/login";
        public static string Logout = "auth/logout";
        public static string Forget_Password = "auth/password/Forget";
        public static string Reset_Password = "auth/password/reset";
        public static string Change_Password = "user/ChangePassword";
        public static string Lock_User = "auth/User/Lock/{id}";
        public static string Unlock_User = "auth/User/Unlock/{id}";


        public static string Role_Create = "role/create";
        public static string Role_Update = "role/update";
        public static string Role_Delete = "role/delete";
        public static string Role_GetById = "role/getById";
        public static string Role_List = "role/getAll";
        public static string Role_ExportToExcel = "role/ExportToExcel";


        public static string Group_Create = "group/create";
        public static string Group_Update = "group/update";
        public static string Group_Delete = "group/";
        public static string Group_GetById = "group/id";
        public static string Group_List = "group/GetGroups";
        public static string Group_ExportToExcel = "group/ExportToExcel";


        public static string User_Create = "user/create";
        public static string user_multiple_insert = "user/multipleCreate";
        public static string user_update = "user/update";
        public static string User_Delete = "user/";
        public static string User_GetById = "user/";
        public static string User_List = "user";
        public static string User_ExportToExcel = "user/ExportToExcel";
        public static string User_UploadImage = "user/uploadImage";



        public static string Config_Domains_Get = "configuration/GetDomainFormat";
        public static string Config_Domains_Post = "configuration/DomainFormat";
        public static string Config_GetEmailKeys = "configuration/GetEmailKeys";
        public static string Config_GetEmailTemplate = "configuration/GetEmailTemplate/";
        public static string Config_UpdateEmailTemplate = "configuration/UpdateEmailTemplate";


        public static string Config_Mail_Get = "configuration/GetMailSettings";
        public static string Config_Mail_Post = "configuration/UpdateMailSettings";
        public static string Config_MaxTrial = "configuration/MaxTrialsLogin";
        public static string Config_PassExpire = "configuration/PasswordExpirationPeriod";

        public static string ResetPassword = "auth/password/reset";
        public static string AccessLog = "Logs/Accesslogs";
        public static string AccessLog_Export = "Logs/ExportAccessToExcel";
        public static string AuditLog = "Logs/Auditlogs";
        public static string AuditLog_Export = "Logs/ExportAuditToExcel";

        public static string LookUp_GetScreens = "LookUps/getScreens";
        public static string AddScreenActionsToRole = "role/AddScreenActionsToRole";

        public static string GetMenu = "screen/MainMenus";
        public static string AllowedActions = "screen/GetScreenPermissions/{id}";
        public static string GetScreenActionsOfRole = "role/GetScreenActionsOfRole/";
    }
}
