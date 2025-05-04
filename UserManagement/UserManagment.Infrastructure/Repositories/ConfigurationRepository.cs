using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagment.Infrastructure.Repositories
{
    public class ConfigurationRepository(UserManagmentDbContext ctx) : IConfigurationRepository
    {
        public async Task<Configuration> GetByKeyAsync(string key)
        {
            var res = await ctx.Configurations.AsNoTracking()
                .Where(e => e.ConfigKey == key).FirstOrDefaultAsync();
            return res;
        }

        public async Task<List<Configuration>> GetByTypeAsync(string type)
        {
            var res = await ctx.Configurations.AsNoTracking()
                .Where(e => e.ConfigType == type).ToListAsync();
            return res;
        }

        public async Task<Configuration> GetByKeyAndTypeAsync(string key, string type)
        {
            var result = await ctx.Configurations
                .Where(e => e.ConfigKey == key && e.ConfigType == type).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Configuration?> InsertKeyValueAsync(Configuration configuration)
        {
            _ = await ctx.Configurations.AddAsync(configuration);
            await ctx.SaveChangesAsync();
            return configuration;
        }

        public async Task<int> SetKeyValue(Configuration configuration, CancellationToken cancellationToken)
        {
            var model = await ctx.Configurations.Where(e => e.ConfigKey == configuration.ConfigKey).FirstOrDefaultAsync(cancellationToken);
            model.ConfigValue = configuration.ConfigValue;
            model.UpdatedDate = DateTime.Now;
            await ctx.SaveChangesAsync(cancellationToken);
            return 1;
        }

        // Get all configurations with ConfigType "email"
        public async Task<List<Configuration>> GetEmailConfigurationsAsync()
        {
            var emailConfigurations = await ctx.Configurations.AsNoTracking()
                .Where(e => e.ConfigType == "email").ToListAsync();
            return emailConfigurations;
        }
    }
}