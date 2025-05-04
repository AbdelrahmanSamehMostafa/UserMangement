namespace UserManagment.Application.SystemConfiguration
{
    public struct ExportExeclConfig
    {
        public static CommonProperties GroupConfig { get; set; }
        public static CommonProperties UserConfig { get; set; }
        public static CommonProperties RoleConfig { get; set; }
        public static CommonProperties AccessLogConfig { get; set; }
        public static CommonProperties AuditLogConfig { get; set; }
        static ExportExeclConfig()
        {
            GroupConfig = new CommonProperties()
            {
                Title = "Groups",
                ReportFileName = "Groups",
                Columns = new List<string> { "Group Name","Group Code", "Number of Users", "Number of Roles" },
                Properties = new List<string> { "Name","Code", "NumberofUsers", "NumberofRoles" },
            };

            UserConfig = new CommonProperties()
            {
                Title = "Users",
                ReportFileName = "Users",
                Columns = new List<string> { "FirstName", "LastName", "Email", "MobileNumber", "DateOfBirth", "Location", "UserName", "Groups" },
                Properties = new List<string> { "FirstName", "LastName", "Email", "MobileNumber", "DateOfBirth", "Location", "UserName", "Groups" },
            };
            RoleConfig = new CommonProperties()
            {
                Title = "Roles",
                ReportFileName = "Roles",
                Columns = new List<string> { "Role Name" },
                Properties = new List<string> { "Name" },
            };
            AccessLogConfig = new CommonProperties()
            {
                Title = "Access Logs",
                ReportFileName = "AccessLogs",
                Columns = new List<string> { "User Name", "Email", "Access Type", "Status", "Date", "Time" },
                Properties = new List<string> { "UserFullName", "EmailAddress", "AccessType", "Status", "Date", "Time" },
            };
            AuditLogConfig = new CommonProperties()
            {
                Title = "Audit Logs",
                ReportFileName = "AuditLogs",
                Columns = new List<string> { "User Name", "Email", "Action", "Log Type","Date", "Time","Status" },
                Properties = new List<string> { "UserFullName", "Email", "logName", "logsType","Date", "time","Status" },
            };
        }
    }
    public struct CommonProperties
    {
        public CommonProperties()
        {
            ReportFileName = string.Empty;
            HeaderInsertionAtZero = string.Empty;
            HeaderInsertionAtOne = string.Empty;
            Title = string.Empty;
            Columns = new List<string>();
            Properties = new List<string>();
        }

        public string ReportFileName { get; set; }
        public string HeaderInsertionAtZero { get; set; }
        public string HeaderInsertionAtOne { get; set; }
        public string Title { get; set; }
        public List<string> Columns { get; set; }
        public List<string> Properties { get; set; }
    }
}