using MediatR;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.GroupDTO;
using UserManagment.Common.DTO.SearchInputs;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Group
{
    public record GroupExportDTO(GroupInputSearch GroupInputSearch) : IRequest<(MemoryStream, string, string)>;

    public class GroupExportHandler : IRequestHandler<GroupExportDTO, (MemoryStream, string, string)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Domain.Models.Group> _logger;

        public GroupExportHandler(IUnitOfWork unitOfWork, ILogger<Domain.Models.Group> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<(MemoryStream, string, string)> Handle(GroupExportDTO request, CancellationToken cancellationToken)
        {
            var _data = await _unitOfWork.Group.GetGroupsAsync(request.GroupInputSearch, cancellationToken);
            if (!_data.Groups.Any())
            {
                _logger.LogError("No groups found");
                throw new CustomException(ErrorResponseMessage.NotFound);
            }

            // Get all role and user counts at once, outside of the loop
            var groupIds = _data.Groups.Select(g => g.Id).ToList();

            var roleCounts = await _unitOfWork.GroupRole.GetRoleCountsByGroupIds(groupIds);
            var userCounts = await _unitOfWork.GroupUser.GetUserCountsByGroupIds(groupIds);

            List<GroupsForExportDTO> groupsForExportDTO = new List<GroupsForExportDTO>();

            foreach (var item in _data.Groups)
            {
                groupsForExportDTO.Add(new GroupsForExportDTO
                {
                    Name = item.Name,
                    Code = item.Code,
                    NumberofRoles = roleCounts.FirstOrDefault(rc => rc.GroupId == item.Id)?.RolesCount ?? 0,
                    NumberofUsers = userCounts.FirstOrDefault(uc => uc.GroupId == item.Id)?.UsersCount ?? 0
                });

            }
            groupsForExportDTO.ToList();

            var memoryStream = await ExportExcel.GenerateExcelAsync(groupsForExportDTO, ExportExeclConfig.GroupConfig.Columns, ExportExeclConfig.GroupConfig.Properties,
            ExportExeclConfig.GroupConfig.ReportFileName, _logger);
            var (contentType, fileName) = ResponseHelper.GetExcelFileDetails("AuditLogs");
            return (memoryStream, contentType, fileName);
        }
    }
}