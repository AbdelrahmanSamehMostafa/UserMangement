using MediatR;
using Microsoft.Extensions.Logging;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.Common;
using UserManagment.Common.Helpers;

namespace UserManagment.Application.Roles
{
    public record RoleExportDTO(BaseListingInput InputSearch) : IRequest<(MemoryStream, string, string)>;

    public class RoleExportHandler : IRequestHandler<RoleExportDTO, (MemoryStream, string, string)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<Domain.Models.Role> _logger;

        public RoleExportHandler(IUnitOfWork unitOfWork, ILogger<Domain.Models.Role> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<(MemoryStream, string, string)> Handle(RoleExportDTO request, CancellationToken cancellationToken)
        {
            var _data = await _unitOfWork.Role.GetRolesAsync(request.InputSearch, cancellationToken);
            if (!_data.Rolses.Any())
            {
                throw new CustomException(ErrorResponseMessage.NotFound);
            }
            var roles = _data.Rolses.ToList();

            var memoryStream = await ExportExcel.GenerateExcelAsync(roles, ExportExeclConfig.RoleConfig.Columns, ExportExeclConfig.RoleConfig.Properties,
            ExportExeclConfig.RoleConfig.ReportFileName, _logger);

            var (contentType, fileName) = ResponseHelper.GetExcelFileDetails("AuditLogs");
            return (memoryStream, contentType, fileName);

        }
    }
}