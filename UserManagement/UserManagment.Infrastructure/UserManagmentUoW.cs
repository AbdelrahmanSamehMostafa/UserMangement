using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using UserManagment.Application.Abstractions;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.LogsDTO;
using UserManagment.Common.Enum;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure;

public class UserManagmentUoW : IUnitOfWork
{
    private readonly UserManagmentDbContext _context;
    private readonly IConfiguration _Configuration;
    public IUserRepository Authentication { get; }
    public IConfigurationRepository Configuration { get; }
    public IRoleRepository Role { get; }
    public IRoleScreenActionRepository RoleScreenAction { get; }
    public IScreenRepository Screen { get; }
    public IScreenActionRepository ScreenAction { get; }
    public IGroupRepository Group { get; }
    public IGroupRoleRepository GroupRole { get; }
    public IGroupUserRepository GroupUser { get; }
    public IAccessLogRepository AccessLog { get; }
    public IAttachmentRepository Attachment { get; }
    public IAuditLogRepository AuditLog { get; }
    public ICurrentSessionProvider CurrentSessionProvider { get; }

    public UserManagmentUoW(UserManagmentDbContext context,
         IUserRepository authentication,
         IConfigurationRepository configuration,
         IRoleRepository role,
         IGroupRepository group,
         IGroupUserRepository groupUser,
         IAccessLogRepository accessLog,
         IScreenActionRepository screenAction,
         IGroupRoleRepository groupRole,
         IRoleScreenActionRepository roleScreenAction,
         IScreenRepository screen,
         ICurrentSessionProvider currentSessionProvider, IConfiguration _configuration,IAuditLogRepository auditLog,IAttachmentRepository attachment)

    {
        _context = context;
        CurrentSessionProvider = currentSessionProvider;
        _Configuration = _configuration;

        Authentication = authentication;
        Configuration = configuration;
        Attachment = attachment;

        AuditLog = auditLog;
        AccessLog = accessLog;

        ScreenAction = screenAction;
        Screen = screen;

        Role = role;
        RoleScreenAction = roleScreenAction;


        GroupRole = groupRole;
        GroupUser = groupUser;
        Group = group;
    }



    public async Task StartTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task SubmitTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        var userId = CurrentSessionProvider.GetUserId();
        SetAuditableProperties(userId);

        var auditEntries = HandleAuditingBeforeSaveChanges(userId).ToList();
        if (auditEntries.Any())
        {
            await _context.AuditTrails.AddRangeAsync(auditEntries, cancellationToken);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Log the exception and inner exception details
            var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            throw new Exception($"An error occurred while saving changes: {innerException}", ex);
        }
    }
    public async Task RevertTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }
    private void SetAuditableProperties(Guid? userId)
    {
        foreach (var entry in _context.ChangeTracker.Entries<BaseModel>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.InsertedDate = DateTime.UtcNow;
                    entry.Entity.InsertedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
    private List<AuditTrail> HandleAuditingBeforeSaveChanges(Guid? userId)
    {
        var data = _Configuration.GetSection("AuditSettings").Get<AuditSettings>();

        var auditableEntries = _context.ChangeTracker.Entries<BaseModel>()
            .Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .Where(x => !data.TablesToSkip.Contains(x.Metadata.GetTableName()))
            .Select(x => CreateTrailEntry(userId, x))
            .ToList();

        return auditableEntries;
    }
    private static AuditTrail CreateTrailEntry(Guid? userId, EntityEntry<BaseModel> entry)
    {
        var trailEntry = new AuditTrail
        {
            Id = Guid.NewGuid(),
            EntityName = entry.Entity.GetType().Name,
            UserId = userId,
            Date = DateTime.Now
        };

        SetAuditTrailPropertyValues(entry, trailEntry);
        SetAuditTrailNavigationValues(entry, trailEntry);

        return trailEntry;
    }
    private static void SetAuditTrailPropertyValues(EntityEntry entry, AuditTrail trailEntry)
    {
        // Skip temp fields (that will be assigned automatically by EF Core engine, for example: when inserting an entity
        foreach (var property in entry.Properties.Where(x => !x.IsTemporary))
        {
            if (property.Metadata.IsPrimaryKey())
            {
                trailEntry.PrimaryKey = property.CurrentValue?.ToString();
                continue;
            }

            if (property.Metadata.Name.Equals("InsertedDate") || property.Metadata.Name.Equals("UpdatedDate"))
            {
                continue;
            }

            SetAuditTrailPropertyValue(entry, trailEntry, property);
        }
    }

    private static void SetAuditTrailPropertyValue(EntityEntry entry, AuditTrail trailEntry, PropertyEntry property)
    {
        var propertyName = property.Metadata.Name;

        // Skip if the property value is null
        if (property.CurrentValue == null && property.OriginalValue == null)
        {
            return;
        }

        switch (entry.State)
        {
            case EntityState.Added:
                // Skip if the current value is null or if the column is 'IsDeleted'
                if (property.CurrentValue != null && propertyName != "IsDeleted")
                {
                    trailEntry.LogsType = LogsType.Create;
                    trailEntry.LogName = LogsType.Create.ToString();
                    trailEntry.NewValues[propertyName] = property.CurrentValue;
                }
                break;

            case EntityState.Deleted:
                // Skip if the original value is null
                if (property.OriginalValue != null)
                {
                    trailEntry.LogsType = LogsType.Delete;
                    trailEntry.LogName = LogsType.Delete.ToString();
                    trailEntry.OldValues[propertyName] = property.OriginalValue;
                }
                break;

            case EntityState.Modified:
                if (property.IsModified &&
                   ((property.OriginalValue != null && !property.OriginalValue.Equals(property.CurrentValue)) ||
                    (property.OriginalValue == null && property.CurrentValue != null)))
                {
                    trailEntry.LogsType = LogsType.Update;
                    trailEntry.LogName = LogsType.Update.ToString();
                    trailEntry.OldValues[propertyName] = property.OriginalValue;
                    trailEntry.NewValues[propertyName] = property.CurrentValue;
                }
                break;
        }


    }



    private static void SetAuditTrailNavigationValues(EntityEntry entry, AuditTrail trailEntry)
    {
        foreach (var navigation in entry.Navigations.Where(x => x.Metadata.IsCollection && x.IsModified))
        {
            if (navigation.CurrentValue is not IEnumerable<object> enumerable)
            {
                continue;
            }

            var collection = enumerable.ToList();
            if (collection.Any())
            {
                continue;
            }
        }
    }

}