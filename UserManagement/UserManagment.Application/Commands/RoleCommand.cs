using MediatR;
using UserManagment.Application.RoleRecords;
using UserManagment.Common.DTO.RoleDTo;

namespace UserManagment.Application.Commands
{

    public class UpdateRoleCommand : IRequest<RoleResultDto>
    {
        public RoleDto roleDto { get; set; }
    }
}
