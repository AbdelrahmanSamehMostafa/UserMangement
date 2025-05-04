using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserManagment.Domain.Models;

namespace UserManagment.Infrastructure;

public partial class UserManagmentDbContext : DbContext
{
    public UserManagmentDbContext()
    {
    }

    public UserManagmentDbContext(DbContextOptions<UserManagmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<AccessLog> AccessLogs { get; set; }
    public virtual DbSet<AuditTrail> AuditTrails { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Screen> Screens { get; set; }
    public virtual DbSet<ScreenAction> ScreenAction { get; set; }
    public virtual DbSet<RoleScreenAction> RoleScreenActions { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<GroupUser> GroupUsers { get; set; }
    public virtual DbSet<GroupRole> GroupRoles { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        // Apply global query filter to all entities inheriting from BaseModel
        modelBuilder.Entity<User>()
            .HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<Configuration>()
        .HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<Role>()
        .HasQueryFilter(m => !m.IsDeleted);


        modelBuilder.Entity<RoleScreenAction>()
          .HasQueryFilter(m => !m.IsDeleted);
        modelBuilder.Entity<Screen>()
          .HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<ScreenAction>()
          .HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<Group>()
        .HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<GroupRole>()
        .HasQueryFilter(m => !m.IsDeleted);

        modelBuilder.Entity<GroupUser>()
        .HasQueryFilter(m => !m.IsDeleted);
         // Configure AuditTrail entity
        modelBuilder.Entity<AuditTrail>(entity =>
        {
            // Convert Dictionary to JSON string for OldValues
            var dictionaryConverter = new ValueConverter<Dictionary<string, object?>, string>(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = true }),
                v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, object?>()
            );

            entity.Property(e => e.OldValues)
                  .HasConversion(dictionaryConverter);

            entity.Property(e => e.NewValues)
                  .HasConversion(dictionaryConverter);            
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
