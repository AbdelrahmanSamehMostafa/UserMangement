namespace UserManagment.Application.Abstractions.DataAbstractions;

public interface IUnitOfWork
{
    IUserRepository Authentication { get; }
    IConfigurationRepository Configuration { get; }
    IRoleRepository Role { get; }
    IRoleScreenActionRepository RoleScreenAction { get; }
    IScreenRepository Screen { get; }
    IScreenActionRepository ScreenAction { get; }
    IGroupRepository Group { get; }
    IGroupRoleRepository GroupRole { get; }
    IGroupUserRepository GroupUser { get; }
    IAccessLogRepository AccessLog { get; }
    IAuditLogRepository AuditLog { get; }
    IAttachmentRepository Attachment { get; }

    Task StartTransactionAsync(CancellationToken cancellationToken);
    Task SubmitTransactionAsync(CancellationToken cancellationToken);
    Task RevertTransactionAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}