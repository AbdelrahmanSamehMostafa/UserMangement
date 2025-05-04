using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UserManagment.Application.Abstractions;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.Identity;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                setInterfaces(cfg);
                setHandlers(cfg);
            });
            services.AddScoped<IdentityService>();
            services.AddScoped<TokenAuthentication>();
            services.AddScoped<ICurrentSessionProvider, CurrentSessionProvider>();

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            return services;
        }

        private static void setHandlers(MediatRServiceConfiguration cfg)
        {
            //user
            cfg.RegisterServicesFromAssembly(typeof(LoginForDevelopmentHandler).Assembly);
            //configuration
            cfg.RegisterServicesFromAssembly(typeof(MaxTrialsLoginHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(PasswordExpirationPeriodHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(DomainFormatHandler).Assembly);
            //Role
            //cfg.RegisterServicesFromAssembly(typeof(RoleCreateHandler).Assembly);
            //cfg.RegisterServicesFromAssembly(typeof(RoleCreateHandler).Assembly);

        }

        private static void setInterfaces(MediatRServiceConfiguration cfg)
        {
            cfg.RegisterServicesFromAssembly(typeof(IUserRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IConfigurationRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IRoleRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IGroupRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IAccessLogRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IAttachmentRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IAuditLogRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IGroupUserRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IGroupRoleRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IScreenRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IScreenActionRepository).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(IRoleScreenActionRepository).Assembly);

        }
    }
}
