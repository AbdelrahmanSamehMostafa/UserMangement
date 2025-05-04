using MediatR;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.UserDTo;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Users
{
    public record UserExportDTO(UserSearchInput ListingInput) : IRequest<(MemoryStream, string, string)>;

    public class UserExportHandler : IRequestHandler<UserExportDTO, (MemoryStream, string, string)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Domain.Models.User> _logger;

        public UserExportHandler(IUnitOfWork unitOfWork, ILogger<Domain.Models.User> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<(MemoryStream, string, string)> Handle(UserExportDTO request, CancellationToken cancellationToken)
        {
            // Retrieve user data
            var _data = await _unitOfWork.Authentication.GetUsersAsync(request.ListingInput, cancellationToken);
            if (!_data.Users.Any())
            {
                _logger.LogWarning("No users found for export");
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            // Collect all user IDs first
            var userIds = _data.Users.Select(u => u.Id).ToList();

            // Get all groups for all users in one go
            var groupsForUsers = await _unitOfWork.GroupUser.GetGroupsForExportAsync(userIds, cancellationToken);

            List<UserBaseListDto> users = new List<UserBaseListDto>();

            // Loop through each user and map their details
            foreach (var item in _data.Users)
            {
                // Get groups as a string for each user
                var userGroups = groupsForUsers
                    .Where(g => g.UserId == item.Id)
                    .Select(g => g.Groups) // Extract the group names
                    .FirstOrDefault(); // Since there will be only one entry per user

                // Map UserListDto object
                users.Add(new UserListDto
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    MobileNumber = item.MobileNumber,
                    DateOfBirth = item.DateOfBirth,
                    Location = item.Location,
                    UserName = item.UserName,
                    Groups = userGroups // If no groups found, return empty
                });
            }

            // Convert list to stream for Excel export
            var memoryStream = await ExportExcel.GenerateExcelAsync(users,
                ExportExeclConfig.UserConfig.Columns,
                ExportExeclConfig.UserConfig.Properties,
                ExportExeclConfig.UserConfig.ReportFileName, _logger);
            var (contentType, fileName) = ResponseHelper.GetExcelFileDetails("AuditLogs");
            return (memoryStream, contentType, fileName);
        }


    }
}