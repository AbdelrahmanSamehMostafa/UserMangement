using UserManagment.Infrastructure.Repositories;
using UserManagment.Application.Abstractions.DataAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using UserManagment.Domain.Models;
using UserManagment.Application.Abstractions;

namespace UserManagment.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserManagmentDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddScoped<IAccessLogRepository, AccessLogRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IUnitOfWork, UserManagmentUoW>();
            

            services.AddScoped<IScreenActionRepository, ScreenActionRepository>();
            services.AddScoped<IScreenRepository, ScreenRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleScreenActionRepository, RoleScreenActionRepository>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IGroupRoleRepository, GroupRoleRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupUserRepository, GroupUserRepository>();





            return services;
        }
    }
}
