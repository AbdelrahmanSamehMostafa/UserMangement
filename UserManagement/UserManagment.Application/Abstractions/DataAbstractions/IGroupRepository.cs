using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.DTO.SearchInputs;

namespace UserManagment.Application.Abstractions.DataAbstractions
{
    public interface IGroupRepository
    {
        Task<bool> IsFound(GroupDTO groupDTO);
        Task<Domain.Models.Group?> Create(Domain.Models.Group group);
        Task<Domain.Models.Group?> Update(Domain.Models.Group groupDTO);
        Task<Domain.Models.Group?> GetById(Guid id);
        Task<List<(string Code, Guid Id)>> GetGroupIdsByCodesAsync(List<string> groupCodes);
        Task<IEnumerable<GroupsForExportDTO>> GetGroupsForExport(GroupInputSearch baseListingInput, CancellationToken cancellationToken);
        Task<(IEnumerable<GroupDTO> Groups, int Count)> GetGroupsAsync(GroupInputSearch baseListingInput, CancellationToken cancellationToken);
    }
}
