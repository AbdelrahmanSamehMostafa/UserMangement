using UserManagment.Domain.Models;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetByKeyAsync(string key);
        Task<Configuration> GetByKeyAndTypeAsync(string key, string type);
        Task<Configuration?> InsertKeyValueAsync(Configuration configuration);
        Task<int> SetKeyValue(Configuration configuration, CancellationToken cancellationToken);
        Task<List<Configuration>> GetByTypeAsync(string type);
        Task<List<Configuration>> GetEmailConfigurationsAsync();
    }
}
