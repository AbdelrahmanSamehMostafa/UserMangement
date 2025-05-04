namespace UserManagmentRazor.Helpers.Enums
{
    public enum LockStatus
    {
        All,
        locked,
        unlocked,
    }

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

    public enum ConfigurationType
    {
        email,
        PasswordExpirationPeriod,
        DomainFormat_Type,
        emailtemplate,
        MaxTrial
    }

    public enum AttachmentType
    {
        User,
        Group,
    }
}
