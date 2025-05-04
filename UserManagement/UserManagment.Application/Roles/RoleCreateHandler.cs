using MediatR;
using UserManagment.Application.Abstractions.DataAbstractions;
using UserManagment.Application.RoleRecords;
using UserManagment.Common.DTO.RoleDTo;
using UserManagment.Common.Helpers;
using UserManagment.Domain.Models;

namespace UserManagment.Application.Roles
{
    public record RoleRequestDTO(string Name, bool IsDefault) : IRequest<RoleResultDto>;

    public class RoleCreateHandler : IRequestHandler<RoleRequestDTO, RoleResultDto>
    {

        private readonly IUnitOfWork _unitOfWork;

        public RoleCreateHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleResultDto> Handle(RoleRequestDTO request, CancellationToken cancellationToken)
        {

            RoleDto roleDto = new RoleDto(request.Name, Guid.Empty, request.IsDefault);
            bool isFound = await _unitOfWork.Role.IsFound(roleDto);
            bool DefaultExist = await _unitOfWork.Role.DefaultRoleExist();
            if ((DefaultExist &&request.IsDefault)|| isFound)
            {
                throw isFound? new CustomException(ErrorResponseMessage.isRepeated): new CustomException(ErrorResponseMessage.Role_Default);
            }
            else
            {
                var newRole = new Role
                {
                    Name = request.Name,
                    Id = Guid.Empty,
                    IsDefault = request.IsDefault
                };
                var res = await _unitOfWork.Role.Create(newRole);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var roleResultDto = RoleMapping.ToRoleResult(res);
                return roleResultDto;
            }
        }
    }
}
