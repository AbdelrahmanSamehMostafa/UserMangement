namespace UserManagment.Common.Enum
{
    public enum ActionType
    {
        List,
        Create,
        Update,
        Delete,
        Export,
        View,
        Reset,
        Lock,
        Unlock,
        Import
    }
    public enum AttachmentType
    {
        User,
        Group,
    }

    public enum AccessStatus
    {
        LoginSuccess,
        LoginFailed,
        LoginPasswordExpired,
        LoginAccountLocked,
        LoginAccountNotActive,
        LogOutSuccess
    }
    public enum UserType
    {
        User,
        Admin,
    }
    public enum ClaimType
    {
        UserId,
        AuthZ,
    }

    public enum LockStatus
    {
        All,
        locked,
        unlocked,
    }

    public enum LogsType : byte
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3,
        Export = 4,
    }
}
